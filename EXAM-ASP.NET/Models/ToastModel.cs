namespace EXAM_ASP_NET.Models
{
    public enum ToastType
    {
        danger,
        info,
        success,
        warning
    }
    public class ToastModel(string message = "Action completed successfully",
                      ToastType type = ToastType.success)
    {
        public string Message { get; set; } = message;
        public ToastType Type { get; set; } = type;
    }
}