/*
MIT License

Copyright (c) 2025 Diego Ramirez

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


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
        public async Task<IActionResult> CreatePayment([FromBody] CardPaymentData request)
        {
            if (request == null)
                return BadRequest("Datos de pago inv�lidos.");

            if (request == null || string.IsNullOrWhiteSpace(request.IdToken))
                return Unauthorized("Token de Google es requerido.");

            try
            {
                var paymentData = new CardPaymentData
                {
                    SourceId = request.SourceId,
                    Amount = request.Amount,
                    Name = request.Name,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    Description = request.Description,
                    Currency = request.Currency,
                    DeviceSessionId = request.DeviceSessionId,
                    IdToken = request.IdToken 

                };

                var chargeId = await _paymentService.CreateCardPaymentAsync(paymentData, request.IdToken);

                return Ok(new { chargeId });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
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