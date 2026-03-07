using Microsoft.Data.SqlClient;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysCountryEV : WebGridEV
    {
        public XysCountryEV()
        {
            MyPageType = "XysCountry";
        }

        public override void InitialViewData()
        {
            ViewPart.UIControl = new UIControl
            {
                UIMode = string.IsNullOrEmpty(PartialData) ? UIModes.@New : UIModes.Edit
            };

            ViewPart.UIControl.Set(new[]
            {
            new UIControl.Item { Name = "CountryId", Label = "CountryId", IsKey = true, IsVisible = false, ValueType = ValueTypes.Defalt },
            new UIControl.Item { Name = "CountryName", Label = "CountryName", Styles = "width:200px;padding-left:4px;", Attributes = "maxlength:100;", IsRequired = true, ValueType = ValueTypes.Defalt, LineSpacing = 1 },
            new UIControl.Item { Name = "CountryAlias", Label = "CountryAlias", Styles = "width:200px;padding-left:4px;", Attributes = "maxlength:20;", ValueType = ValueTypes.Defalt, LineSpacing = 1 },
            new UIControl.Item { Name = "CountryOrder", Label = "CountryOrder", Styles = "width:80px;padding-left:4px;", Attributes = "maxlength:5;", UIType = UITypes.Number, ValueType = ValueTypes.Defalt, InitialValues = "1", LineSpacing = 20 }
        });

            string SSQL = " Select CountryId,CountryName,CountryAlias,CountryOrder from XysCountry  where CountryId = N'" + PartialData + "'";
            ViewPart.BindData(SSQL);
        }

        protected override string VerifySave()
        {
            string rtnvlu = string.Empty;
            string CountryName = ViewPart.Field("CountryName").value;

            if (string.IsNullOrEmpty(CountryName))
            {
                rtnvlu = "msg_required";
            }
            return rtnvlu;
        }

        protected override string PutSaveData()
        {
            List<string> SQL = new List<string>();

            if (string.IsNullOrEmpty(ViewPart.Field("CountryId").value))
            {
                ViewPart.Field("CountryId").value = NewID();
                SQL.Add(" Insert into XysCountry(CountryId,CountryName,CountryAlias,CountryOrder,SYSDTE,SYSUSR) " +
                        " values( @CountryId,@CountryName,@CountryAlias,@CountryOrder, getdate(), @SYSUSR) ");
            }
            else
            {
                SQL.Add(" Update XysCountry set " +
                        " CountryName = @CountryName, CountryAlias = @CountryAlias,CountryOrder=@CountryOrder, " +
                        " SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                        " WHERE CountryId = @CountryId");
            }

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@CountryId", Value = ViewPart.Field("CountryId").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@CountryName", Value = ViewPart.Field("CountryName").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@CountryAlias", Value = ViewPart.Field("CountryAlias").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@CountryOrder", Value = ViewPart.Field("CountryOrder").value, SqlDbType = SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        protected override string VerifyDelete()
        {
            return string.Empty;
        }

        protected override string PutDeleteData()
        {
            List<string> SQL = new List<string>
        {
            " delete from XysCountry WHERE CountryId = @CountryId "
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@CountryId", Value = ViewPart.Field("CountryId").value, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }
    }
}
