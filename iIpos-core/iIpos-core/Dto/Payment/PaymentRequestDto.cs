namespace iIpos_core.Dto.Payment
{
    public class PaymentRequestDto
    {
        public string Token { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
    }
}
