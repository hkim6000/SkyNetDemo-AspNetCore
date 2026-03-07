using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysCountry: WebGrid
    {
        public XysCountry() {
            MyPageType = this.GetType().Name;
        }
    }
}
