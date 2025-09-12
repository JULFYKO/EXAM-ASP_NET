namespace EXAM_ASP_NET.Models
{
    public enum DialogType
    {
        danger,
        info,
        success,
        warning
    }
    public class ConfirmDialogModel
    {
        public string DialogId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public DialogType Mode { get; set; } = DialogType.info;
    }
}