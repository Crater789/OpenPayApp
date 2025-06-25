using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OpenPayApi.Dto;
using System.Threading.Tasks;

namespace OpenPayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("pagar")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = new CardPaymentData
            {
                Name = dto.Name,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Amount = dto.Amount,
                CardNumber = dto.CardNumber,
                Cvv2 = dto.Cvv2,
                ExpirationMonth = dto.ExpirationMonth,
                ExpirationYear = dto.ExpirationYear,
                HolderName = dto.HolderName,
                DeviceSessionId = dto.DeviceSessionId
    
            };

            try
            {
                var chargeId = await _paymentService.CreateCardPaymentAsync(data);
                return Ok(new { chargeId });
            }
            catch (Exception ex)
            {
                // Aquí puedes loggear ex.Message o ex.StackTrace para depuración
                return StatusCode(500, new { message = "Error procesando el pago", details = ex.Message });
            }
        }
    }
}
