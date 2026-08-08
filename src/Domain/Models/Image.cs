namespace Domain.Models
{
    public record Image(string FileName, Uri? Url = null!)
    {
        protected Image() : this(string.Empty!) { }
    }
}
