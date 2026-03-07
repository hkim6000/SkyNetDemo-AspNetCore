using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using static SkyNet.ToolKit.SQLGridSection;
using static SkyNet.WebPage;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysCountryMV : WebGridMV
    {
        public XysCountryMV()
        {
            SQLGridPage = this.GetType().Name;
            SQLGridFilter = "CountryName + CountryAlias";
            SQLGridOrderBy = new string[0];
            string GridName = Translator.Format("title");
            string GridTables = "XysCountry";

            UIGrid UIGrid = new UIGrid();
            UIGrid.Items.AddRange(new UIGrid.Item[]
            {
                new UIGrid.Item { Name = "Edit", Value = "'&#10148;'", Label = string.Empty },
                new UIGrid.Item { Name = "CountryId", Label = "CountryId", IsKey = true },
                new UIGrid.Item { Name = "CountryName", Label = "CountryName" },
                new UIGrid.Item { Name = "CountryAlias", Label = "CountryAlias" },
                new UIGrid.Item { Name = "CountryOrder", Label = "CountryOrder" },
                new UIGrid.Item { Name = "SYSDTE", Label = "SYSDTE" },
                new UIGrid.Item { Name = "SYSUSR", Value = "dbo.XF_UserName(SYSUSR)", Label = "SYSUSR" }
            });

            SQLGridInfo.Name = GridName;
            SQLGridInfo.Id = this.GetType().Name;

            int pageNoVal = (int)Common.Val(ParamValue(SQLGridInfo.Id + "_PageNo"));
            SQLGridInfo.CurrentPageNo = (int)(pageNoVal == 0 ? 1 : pageNoVal);

            SQLGridInfo.LinesPerPage = 30;
            SQLGridInfo.ExcludeDownloadColumns = new int[] { 0, 1 };
            SQLGridInfo.DisplayCount = SQLGridSection.DisplayCounts.FilteredOnly;
            SQLGridInfo.TitleEnabled = true;

            SQLGridInfo.Query = new SQLGridSection.SQLQuery
            {
                Tables = GridTables,
                OrderBy = UIGrid.Items.FindAll(x => x.IsKey == true).Select(x => x.Name).ToArray(),
                Columns = UIGrid.Columns().ToArray(),
                ColumnAlias = UIGrid.Labels().ToArray(),
                Filters = string.IsNullOrEmpty(ParamValue(SQLGridInfo.Id + "_Filter"))
                    ? SQLGridFilter + " like '%%' "
                    : ParamValue(SQLGridInfo.Id + "_Filter")
            };
        }
        protected override void SetGridStyle(SQLGridSection SQLGrid)
        {
            SQLGrid.Wrap.SetStyle(HtmlStyles.margin, string.Empty);
            SQLGrid.Wrap.SetStyle(HtmlStyles.marginLeft, "8px");
            SQLGrid.Wrap.SetStyle(HtmlStyles.display, "inline-block");
            SQLGrid.Wrap.SetStyle(HtmlStyles.minWidth, "60%");
            SQLGrid.Wrap.SetStyle(HtmlStyles.overflow, "auto");

            if (SQLGrid.GridData != null)
            {
                ViewMethod editMethod = GetViewMethod("edit");

                SQLGrid.Grid.TableColumns[0].SetColumnStyle(HtmlStyles.cursor, "pointer");
                SQLGrid.Grid.TableColumns[0].SetColumnAttribute(HtmlEvents.onclick, ByPassCall(editMethod.Method, editMethod.Params));

                SQLGrid.Grid.TableColumns[1].SetHeaderStyle(HtmlStyles.display, "none");
                SQLGrid.Grid.TableColumns[1].SetColumnStyle(HtmlStyles.display, "none");

                SQLGrid.Grid.TableColumns[2].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");
                SQLGrid.Grid.TableColumns[3].SetColumnStyle(HtmlStyles.whiteSpace, "nowrap");

                SQLGrid.Grid.TableColumns[5].SetColumnFormat("@D {5} |" + LocalDateFormat);

                for (int i = 0; i < SQLGrid.Grid.TableColumns.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                        case 3:
                        case 4:
                            SQLGrid.Grid.TableColumns[i].SetColumnStyle(HtmlStyles.textAlign, "center");
                            break;
                        default:
                            SQLGrid.Grid.TableColumns[i].SetColumnStyle(HtmlStyles.textAlign, "left");
                            break;
                    }
                }
            }
        }
    }
}
