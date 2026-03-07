using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysArea: WebGrid
    {
        public XysArea() {
            MyPageType = this.GetType().Name;
        }
    }
}
