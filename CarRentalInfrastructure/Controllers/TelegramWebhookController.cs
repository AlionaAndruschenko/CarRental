using Microsoft.AspNetCore.Mvc;
using CarRentalInfrastructure.Services;
using System.Text.Json;
using CarRentalInfrasructure;
using Microsoft.EntityFrameworkCore;

namespace CarRentalInfrastructure.Controllers;

[ApiController]
[Route("webhook/telegram")]
public class TelegramWebhookController : ControllerBase
{
    private readonly TelegramBotService _botService;
    private readonly CarRentalDbContext _context;

    public TelegramWebhookController(TelegramBotService botService, CarRentalDbContext context)
    {
        _botService = botService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> HandleUpdate()
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync();
        var update = JsonDocument.Parse(json);

       
        if (!update.RootElement.TryGetProperty("message", out var message))
            return Ok();

        var chatId = message.GetProperty("chat").GetProperty("id").ToString();
        var text = message.TryGetProperty("text", out var txt) ? txt.GetString() : "";

        if (text == "/start")
        {
            await _botService.SendMessageAsync(chatId,
                "Привіт! 👋\nЩоб переглянути свої оренди, надішліть:\n\n" +
                "<b>ПІБ та номер телефону</b>\n\n" +
                "Наприклад:\nІванов Іван\n+380123456789",
                parseMode: "HTML"
            );
        }
        else if (!string.IsNullOrWhiteSpace(text) && text != "/start")
        {
            var lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length >= 2)
            {
                var fullName = lines[0].Trim();
                var phone = lines[1].Trim();

               
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c =>
                        EF.Functions.Like(c.FullName.ToLower(), $"%{fullName.ToLower()}%") &&
                        c.PhoneNumber.Contains(phone)
                    );

                if (customer != null)
                {
                    var rentals = await _context.Rentals
                        .Where(r => r.CustomerId == customer.Id)
                        .Include(r => r.Car)
                        .ToListAsync();

                    if (rentals.Any())
                    {
                        var msg = "✅ Знайдено ваші оренди:\n\n";
                        foreach (var r in rentals)
                        {
                            msg += $"🚗 <b>{r.Car.Make} {r.Car.Model}</b>\n" +
                                   $"📌 Номер: {r.Car.LicensePlate}\n" +
                                   $"📅 Оренда: {r.RentalDate:dd.MM.yyyy}\n" +
                                   $"📆 Повернення: {r.ReturnDate?.ToString("dd.MM.yyyy") ?? "—"}\n\n";
                        }
                        await _botService.SendMessageAsync(chatId, msg, parseMode: "HTML");
                    }
                    else
                    {
                        await _botService.SendMessageAsync(chatId, "У вас немає активних оренд.");
                    }
                }
                else
                {
                    await _botService.SendMessageAsync(chatId,
                        "❌ Клієнта не знайдено.\n" +
                        "Переконайтеся, що ПІБ та номер телефону введені правильно.\n" +
                        "Спробуйте ще раз.");
                }
            }
            else
            {
                await _botService.SendMessageAsync(chatId,
                    "Надішліть у двох рядках:\n1. ПІБ\n2. Номер телефону\n\nПриклад:\nІванов Іван\n+380123456789");
            }
        }

        return Ok();
    }
}