using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> CreatePayment([FromBody] CardPaymentData payment)
        {
            try
            {
                var chargeId = await _paymentService.CreateCardPaymentAsync(payment);
                return Ok(new { chargeId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error procesando el pago",
                    details = ex.Message
                });
            }
        }

    }
}
