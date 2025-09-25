namespace WcfConsumer.Infrastructure.Decorators
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheableAttribute : Attribute
    {
        public int ExpireMinutes { get; set; } = 60;

        public CacheableAttribute() { }

        public CacheableAttribute(int expireMinutes)
        {
            ExpireMinutes = expireMinutes;
        }
    }
}
