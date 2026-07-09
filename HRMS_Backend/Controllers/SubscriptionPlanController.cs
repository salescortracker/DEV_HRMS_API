using System.Text;
using BusinessLayer.DTOs;
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionPlanController : ControllerBase
    {
        private readonly ISubscriptionPlanService _service;
        private readonly IUserSubscriptionService _userservice;
        private readonly IConfiguration _config;
        private readonly HRMSContext _context;
        private readonly ISubscriptionJobService _subscriptionJobService;
        public SubscriptionPlanController(IUserSubscriptionService userservice,ISubscriptionPlanService service, IConfiguration config, HRMSContext context, ISubscriptionJobService subscriptionJobService)
        {
            _service = service;
            _userservice = userservice;
            _config = config;
            _context = context;
            _subscriptionJobService = subscriptionJobService;
        }

        [HttpGet("GetUserSubscription")]
        public async Task<IActionResult> GetUserSubscription(int userId)
        {
            var result = await _service.GetUserSubscription(userId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto req)
        {
            var plan = await _service.GetPlanByIdAsync(req.PlanId);
            var receipt = $"rcpt_{DateTime.UtcNow.Ticks}";
            receipt = receipt.Length > 40 ? receipt.Substring(0, 40) : receipt;


            if (plan == null)
                return BadRequest("Plan not found");
            if (plan.Price <= 0)
                return BadRequest("Invalid plan price");
            int amount = (int)(plan.Price * 100);

            RazorpayClient client = new RazorpayClient(
                _config["Razorpay:KeyId"],
                _config["Razorpay:KeySecret"]
            );

            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", amount);
            options.Add("currency", "INR");
            options.Add("receipt", receipt);

            Order order = client.Order.Create(options);

            return Ok(new
            {
                orderId = order["id"].ToString(),
                amount = amount,
                key = _config["Razorpay:KeyId"]
            });
        }
        [HttpPost("ActivateSubscription")]
        public async Task<IActionResult> ActivateSubscription([FromBody] ActivateDto dto)
        {
            string keySecret = _config["Razorpay:KeySecret"];

            string payload = dto.OrderId + "|" + dto.PaymentId;

            var expectedSignature = ComputeHmac(payload, keySecret);

            if (expectedSignature != dto.Signature)
                return BadRequest("Invalid payment");

            // 1️⃣ Create Subscription
            var sub = new UserSubscription
            {
                UserId = dto.UserId,
                PlanId = dto.PlanId,
                PaymentId = dto.PaymentId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                Status = "ACTIVE",
                PaymentStatus = "PAID"
            };

            _context.UserSubscriptions.Add(sub);
            await _context.SaveChangesAsync();

            // 2️⃣ 🔥 CREATE INVOICE HERE (IMPORTANT)
            await _subscriptionJobService.CreateInvoiceAsync(
                dto.UserId,
                dto.PlanId,
                dto.PaymentId,
                dto.OrderId
            );

            return Ok("Subscription Activated + Invoice Generated");
        }
        private string ComputeHmac(string data, string secret)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256(
                Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        [HttpPost("CreateInvoice")]
        public async Task<IActionResult> CreateInvoice([FromBody] ActivateDto dto)
        {
            var invoice = await _subscriptionJobService.CreateInvoiceAsync(
                dto.UserId,
                dto.PlanId,
                dto.PaymentId,
                dto.OrderId
            );

            return Ok(invoice);
        }
        [HttpGet("GetInvoiceByUser")]
        public async Task<IActionResult> GetInvoiceByUser(int userId)
        {
            var invoice = await _context.Invoices
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.InvoiceId)
                .FirstOrDefaultAsync();

            return Ok(invoice);
        }

        [HttpGet("GetPlans")]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _service.GetPlansAsync();
            return Ok(plans);
        }

        [HttpGet("GetPlanById/{id}")]
        public async Task<IActionResult> GetPlanById(int id)
        {
            var plan = await _service.GetPlanByIdAsync(id);

            if (plan == null)
                return NotFound();

            return Ok(plan);
        }

        [HttpPost("CreatePlan")]
        public async Task<IActionResult> CreatePlan([FromBody] SubscriptionPlanDto dto)
        {
            var result = await _service.CreatePlanAsync(dto);

            return Ok(result);
        }

        [HttpPut("UpdatePlan/{id}")]
        public async Task<IActionResult> UpdatePlan(int id, [FromBody] SubscriptionPlanDto dto)
        {
            var result = await _service.UpdatePlanAsync(id, dto);

            return Ok(result);
        }

        [HttpDelete("DeletePlan/{id}")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            var result = await _service.DeletePlanAsync(id);

            if (!result)
            {
                return BadRequest(new
                {
                    message = "Plan cannot be deleted because it is already in use or does not exist."
                });
            }

            return Ok(new
            {
                message = "Plan deleted successfully."
            });
        }

        [HttpPost("ApplyPlan")]
        public async Task<IActionResult> ApplyPlan([FromBody] UserSubscriptionDto dto)
        {
            var result = await _userservice.ApplyPlanAsync(dto);

            return Ok(result);
        }
        [HttpPost("SavePlanMenus")]
        public async Task<IActionResult> SavePlanMenus([FromBody] PlanMenuRequestDto request)
        {
            var result = await _service.SavePlanMenus(request);
            return Ok(result);
        }
        [HttpGet("GetAllMenus")]
        public async Task<IActionResult> GetAllMenus()
        {
            var menus = await _service.GetAllMenus();
            return Ok(menus);
        }
        [HttpGet("GetMenusByType/{type}")]
        public async Task<IActionResult> GetMenusByType(string type)
        {
            var menus = await _service.GetMenusByType(type);
            return Ok(menus);
        }

        [HttpGet("GetUserModules/{userId}")]
        public async Task<IActionResult> GetUserModules(int userId)
        {
            var result = await _service.GetUserAllowedModules(userId);

            return Ok(result);
        }
    }
}
