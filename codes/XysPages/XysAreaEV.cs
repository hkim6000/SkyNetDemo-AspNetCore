using Microsoft.Data.SqlClient;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysAreaEV : WebGridEV
    {
        public XysAreaEV()
        {
            MyPageType = "XysArea";
        }

        public override void InitialViewData()
        {
            string InitValue = "SQL@ select a.CountryId, a.CountryName+' ('+ a.CountryAlias+')' from XysCountry a order by a.CountryName";
            string InitValue1 = "{1|AreaFlag|*}";

            ViewPart.UIControl = new UIControl
            {
                KeyLockOnEdit = true,
                UIMode = string.IsNullOrEmpty(PartialData) ? UIModes.@New : UIModes.Edit
            };

            ViewPart.UIControl.Set(new UIControl.Item[]
            {
            new UIControl.Item { Name = "AreaId", Label = "AreaId", Styles = "width:120px;", IsKey = true, IsRequired = true, Attributes = "maxlength:10;", LineSpacing = 1 },
            new UIControl.Item { Name = "AreaName", Label = "AreaName", Attributes = "maxlength:100;", IsRequired = true, LineSpacing = 10 },
            new UIControl.Item { Name = "CountryId", Label = "CountryId", UIType = UITypes.Dropdown, InitialValues = InitValue, LineSpacing = 10 },
            new UIControl.Item { Name = "AreaOrder", Label = "AreaOrder", Styles = "width:80px;padding-left:4px;", Attributes = "maxlength:5;", UIType = UITypes.Number, ValueType = ValueTypes.Defalt, InitialValues = "1", LineSpacing = 10 },
            new UIControl.Item { Name = "AreaFlag", Label = "AreaFlag", UIType = UITypes.Checkbox, InitialValues = InitValue1, LineSpacing = 20 }
            });

            string SSQL = " Select AreaId, AreaName, " +
                          " (select top 1 CountryId from XysCountry where CountryName =(select top 1 CountryName from XysArea where AreaId = N'" + PartialData + "')) as [CountryId], " +
                          "       AreaOrder,AreaFlag " +
                          " from XysArea  where AreaId = N'" + PartialData + "'";

            ViewPart.BindData(SSQL);
        }

        protected override string VerifySave()
        {
            string rtnvlu = string.Empty;

            string AreaId = ViewPart.Field("AreaId").value;
            string AreaName = ViewPart.Field("AreaName").value;

            if (string.IsNullOrEmpty(AreaId) || string.IsNullOrEmpty(AreaName))
            {
                rtnvlu = "msg_required";
            }
            return rtnvlu;
        }

        protected override string PutSaveData()
        {
            List<string> SQL = new List<string>();

            if (ViewPart.Data == null)
            {
                SQL.Add(" Insert into XysArea( AreaId,AreaName,CountryName,CountryAlias,AreaOrder,AreaFlag,SYSDTE,SYSUSR) " +
                        " values ( @AreaId, @AreaName, " +
                        "       (select top 1 CountryName from XysCountry where CountryId = @CountryId), " +
                        "       (select top 1 CountryAlias from XysCountry where CountryId = @CountryId), " +
                        "       @AreaOrder, @AreaFlag, getdate(), @SYSUSR)  ");
            }
            else
            {
                SQL.Add("  Update XysArea set " +
                        " AreaName = @AreaName, " +
                        " CountryName = (select top 1 CountryName from XysCountry where CountryId = @CountryId), " +
                        " CountryAlias = (select top 1 CountryAlias from XysCountry where CountryId = @CountryId), " +
                        " AreaOrder = @AreaOrder, " +
                        " AreaFlag = @AreaFlag, " +
                        " SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                        " WHERE  AreaId = @AreaId ");
            }

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@AreaId", Value = ViewPart.Field("AreaId").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@AreaName", Value = ViewPart.Field("AreaName").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@CountryId", Value = ViewPart.Field("CountryId").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@AreaOrder", Value = ViewPart.Field("AreaOrder").value, SqlDbType = SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@AreaFlag", Value = Convert.ToDouble(ViewPart.Field("AreaFlag").value).ToString(), SqlDbType = SqlDbType.Int });
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
            " delete from XysArea WHERE AreaId = @AreaId "
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@AreaId", Value = ViewPart.Field("AreaId").value, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }
    }

}
