using KyGYS.Controls.Caller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UltraDbEntity;
using MoreLinq;
using KyGYS.Controls;
using System.Data;
using Ultra.Web.Core.Common;
namespace KyGYS.Data
{
    public partial class print : BasicSecurity
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
        }
        [System.Web.Services.WebMethod]
        public static string Print(string guid)
        {
            if (HttpContext.Current.Session["UserName"] == null)
            {
                return "操作失败";
            }
            string userName = HttpContext.Current.Session["UserName"].ToString();
            if (string.IsNullOrEmpty(userName))
            {
                return "操作失败";
            }
            string msg = string.Empty;
            if (string.IsNullOrEmpty(guid)) return msg = "操作失败,请联系客易技术";

            SerNoCaller.Calr_SuppBatch.ExecSql("Update T_ERP_SuppBatch set Reserved1 = 1 where guid =@0 and SuppName = @1", guid, userName);
            var bch = SerNoCaller.Calr_SuppBatch.Get("Where Guid = @0", guid).FirstOrDefault();
            //var ords = SerNoCaller.Calr_SuppOrder.Get("select * from V_ERP_SuppOrder Where SuppBatchGuid = @0", guid);

            var batchlist = SerNoCaller.Calr_V_ERP_NPrintBatch.GetByProc("exec P_ERP_GetNPrintBatch @0,@1", bch.MergerSysNo, bch.BatchGuid).FirstOrDefault();
            var nitemlist = SerNoCaller.Calr_V_ERP_NPrintItem.GetByProc("exec P_ERP_NPrintItem @0,@1", bch.MergerSysNo, bch.BatchGuid);
            if (batchlist == null || nitemlist == null) return msg = "操作失败,请联系客易技术";
            List<V_ERP_NPrintItem> itemlist = null;
            if (userName != "admin")
            {
                itemlist = nitemlist.Where(j => j.SuppName == userName).ToList();
            }
            else
            {
                itemlist = nitemlist;
            }
            string BX_SendDate = batchlist.SellerMemo.Contains("指定日期") ? "指定日期" : (batchlist.SellerMemo.Contains("等通知") ? "等通知" : "3天");
            string BX_GetPointPrice = string.Empty;
            if (null == batchlist.PostFee)
            {
                BX_GetPointPrice = string.Empty;
            }
            else
            {
                BX_GetPointPrice = string.Format("{0:F2} /方", batchlist.PostFee.Value);
            }
            if (batchlist == null || itemlist == null || itemlist.Count < 1) return "";
            int count = itemlist.Count;
            count = count + 1;
            #region 凯森
            //List<UltraDbEntity.T_ERP_SuppOrder> lstSum = new List<T_ERP_SuppOrder>();
            //ords.ForEach(j=>{
            //   j.Num = j.Num * (j.PackageCount== null ? 1 : j.PackageCount.Value == 0 ? 1:j.PackageCount.Value);
            //   while (j.Num > 0)
            //   {
            //       var net = j.Copy(); net.Num = 1;
            //       lstSum.Add(net);
            //       j.Num = j.Num - 1;
            //   }
            //});
            #endregion

            StringBuilder sb = new StringBuilder();
            sb.Append("<div id=\"printdiv\" style=\"width: 720px; height: 500px;\">\n");
            //sb.Append("<span style=\"float: right;\">\n");
            //sb.Append("No:" + "12111");
            //sb.Append("</span >\n");

            DataTable dt = new DataTable();
            string serialNo = string.Empty;
            //var rd = SqlHelper.ExecuteDataTable(SQLCONN.Conn, CommandType.StoredProcedure, "P_ERP_AutoCreatePrintNo");
            //if (rd == null) return "";
            //dt = rd.Copy();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    serialNo = dr["SerialNo"].ToString();
            //}
            //<span style=\"margin-left:5px; font-weight:normal;font-family:'Times New Roman';font-size:13px;\">" + "No:" + serialNo + "</span>
            sb.Append("<div style=\"margin-top:20px;margin-left:80px; width:767px; \"><span style=\"font-family:'新宋体'; font-size:28px; font-weight:bold;\">" + batchlist.BXFHName + "</span></div>");
            sb.Append("<style type=\"text/css\">td{border: solid #000 1px;}</style>");
            sb.Append("<table style=\"width: 740px; margin-left:0px; margin-top:5px; font-size:12px;  border-collapse: collapse;   border: none;\"   border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
            sb.Append("<tr><td  style=\"width:80px;height:29px;line-height:17px;text-align:center;font-family:'微软雅黑';font-size:13px;\" rowspan=\"4\";>订单信息</td><td  style=\"width:15%;height:28px;line-height:17px;text-align:left;vertical-align:middle;padding-left:1px;font-family:'微软雅黑';font-size:13px;\"> 下单日期</td><td style=\"height:29px;line-height:17px;text-align:left;font-family:'微软雅黑';font-size:13px;\" colspan=\"12\">" + batchlist.PayTime + "</td><td style=\"width:84px;height:28px;line-height:17px;text-align:left;vertical-align:middle;padding-left:1px;font-family:'微软雅黑';font-size:13px;\">发货日期</td><td style=\"height:29px;line-height:17px;text-align:left;font-family:'微软雅黑';font-size:13px;\" colspan=\"30\">" + BX_SendDate + "</td></tr>");
            sb.Append("<tr><td  style=\"width:15%;height:28px;line-height:17px;text-align:left;vertical-align:middle;padding-left:1px;font-family:'微软雅黑';font-size:13px;\">下单会员名</td><td style=\"height:29px;line-height:17px;text-align:left;font-family:'微软雅黑';font-size:13px;\" colspan=\"12\" >" + batchlist.BuyerNick + "</td><td style=\"width:84px;height:28px;line-height:17px;text-align:left;vertical-align:middle;padding-left:1px;font-family:'微软雅黑';font-size:13px;\">收件人姓名</td><td style=\"height:29px;line-height:17px;text-align:left;font-family:'微软雅黑';font-size:13px;\" colspan=\"30\">" + batchlist.ReceiverName + "</td></tr>");

            sb.Append("<tr> <td  style=\"width:15%;height:28px;line-height:17px;text-align:left;vertical-align:middle;padding-left:1px;font-family:'微软雅黑';font-size:13px;\">货运目的地</td><td style=\"height:29px;line-height:17px;text-align:left;font-family:'微软雅黑';font-size:13px;\" colspan=\"12\">" + batchlist.RecvStateCity + "</td><td style=\"width:84px;height:28px;line-height:17px;text-align:left;vertical-align:middle;padding-left:1px;font-family:'微软雅黑';font-size:13px;\">收件人电话</td><td style=\"height:29px;line-height:17px;text-align:left;font-family:'微软雅黑';font-size:13px;\" colspan=\"30\">" + batchlist.ReceiverMobile + "</td></tr>");

            sb.Append(" <tr> <td  style=\"width:15%;height:28px;line-height:17px;text-align:left;vertical-align:middle;padding-left:1px;font-family:'微软雅黑';font-size:13px;\">收件人详细地址</td><td style=\"height:29px;line-height:17px;text-align:left;font-family:'微软雅黑';font-size:13px;\" colspan=\"42\">" + batchlist.ReceiverAddress + "</td></tr>");

            sb.Append("<tr  ><td  style=\"width:64px;height:29px;line-height:17px;text-align:center;font-family:'微软雅黑';font-size:13px;\" rowspan=\"3\";>物流信息</td><td  style=\"width:80px;height:28px;line-height:17px;text-align:left;vertical-align:middle;padding-left:1px;font-family:'微软雅黑';font-size:13px;\">物流公司</td><td style=\"height:29px;line-height:17px;text-align:center;font-family:'微软雅黑';font-size:13px;\" colspan=\"12\">" + batchlist.LogisName + "</td><td style=\"width:84px;height:28px;line-height:17px;text-align:left;vertical-align:middle;padding-left:1px;font-family:'微软雅黑';font-size:13px;\">运费</td><td style=\"height:29px;line-height:17px;text-align:left;font-family:'微软雅黑';font-size:13px;\" colspan=\"30\"></td></tr>");

            sb.Append("<tr> <td  style=\"width:15%;height:28px;line-height:17px;text-align:left;vertical-align:middle;padding-left:1px;font-family:'微软雅黑';font-size:13px;\">提货方式</td><td style=\"height:29px;line-height:17px;text-align:left;font-family:'微软雅黑';font-size:13px;\" colspan=\"12\">" + batchlist.ThirdPackType + "</td><td style=\"width:84px;height:28px;line-height:17px;text-align:left;vertical-align:middle;padding-left:1px;font-family:'微软雅黑';font-size:13px;\">运费付款方式</td><td style=\"height:29px;line-height:17px;text-align:left;font-family:'微软雅黑';font-size:13px;\" colspan=\"30\">" + batchlist.PostFeeType + "</td></tr>");

            sb.Append("<tr> <td  style=\"width:247px;height:28px;line-height:17px;text-align:center;vertical-align:middle;font-family:'微软雅黑';font-size:13px;\" colspan=\"4\">有无一起发货商品</td><td colspan=\"35\"><span style=\"text-align:center;vertical-align:middle;font-family:'微软雅黑';font-size:15px;\">&nbsp;&nbsp;<input type=\"checkbox\" name=\"checkbox1\" value=\"value1\"  /> 有</span>&nbsp;<span style=\"text-align:center;vertical-align:middle;font-family:'微软雅黑';font-size:14px;\"><input type=\"checkbox\" name=\"checkbox1\" value=\"value1\" /> 无</span></td></tr>");

            sb.Append(" <tr><td  style=\"width:64px;height:29px;line-height:17px;text-align:center;font-family:'微软雅黑';font-size:13px;\" rowspan=\"" + (itemlist.Count + 1) + "\";>订单详情</td><td colspan=\"4\" style=\"width:10%;height:28px;line-height:17px;text-align:center;vertical-align:middle;font-family:'微软雅黑';font-size:13px;\">货品名称</td><td style=\"text-align:center;vertical-align:middle;font-family:'微软雅黑';font-size:13px;\" colspan=\"9\">方向</td><td style=\"text-align:center;vertical-align:middle;font-family:'微软雅黑';font-size:13px;\" colspan=\"10\">颜色</td><td style=\"text-align:center;vertical-align:middle;font-family:'微软雅黑';font-size:13px;\" colspan=\"13\">尺寸大小</td><td style=\"width:5%; text-align:center;vertical-align:middle;font-family:'微软雅黑';font-size:13px;\" colspan=\"3\">数量</td></tr>");
            foreach (var item in itemlist)
            {
                sb.Append("<tr><td colspan=\"4\" style=\"width:10%;height:28px;line-height:17px;text-align:center;vertical-align:middle;font-family:'微软雅黑';font-size:13px;\">" + item.BXItemName + "</td><td colspan=\"9\" style=\"text-align:center;vertical-align:middle;\">" + item.Direction + "</td><td style=\"text-align:center;vertical-align:middle;font-family:'微软雅黑';font-size:13px;\" colspan=\"10\">" + item.Color + "</td><td colspan=\"13\" style=\"text-align:center;vertical-align:middle;font-family:'微软雅黑';font-size:13px;\">" + item.Remark + "</td><td style=\"width:5%;text-align:center;vertical-align:middle;font-family:'微软雅黑';font-size:13px;\" colspan=\"3\">" + item.Num + "</td></tr>");
            }
            sb.Append("<tr  ><td  style=\"width:64px;height:35px;line-height:17px;text-align:center;font-family:'微软雅黑';font-size:13px;\" rowspan=\"1\";>附加说明</td><td colspan=\"43\" rowspan=\"1\" style=\"width:345px;height:28px;line-height:17px;text-align:center;vertical-align:middle;font-family:'微软雅黑';font-size:13px;\">" + batchlist.AddrPrintMemo + "</td></tr>");

            sb.Append("<tr><td  style=\"width:64px;height:29px;line-height:17px;text-align:center;font-family:'微软雅黑';font-size:13px;\" rowspan=\"2\";>公司信息</td><td colspan=\"43\" style=\"width:400px;height:28px;line-height:17px;text-align:left;vertical-align:middle;font-family:'微软雅黑';font-size:13px;\">地址：广东省佛山市顺德区龙江镇龙头家具材料城A3座301-303</td></tr>");
            sb.Append(" <tr><td colspan=\"43\" style=\"width:345px;height:28px;line-height:17px;text-align:left;vertical-align:middle;font-family:'微软雅黑';font-size:13px;\">联系电话：18942483552</td></tr>");
            sb.Append("</table>");
            sb.Append("<br/>");
            sb.Append("<span style=\"border:solid #FFFFFF 1px;text-align:center;vertical-align:middle; width:120px;  font-family:'微软雅黑';font-size:16px;font-weight:bold;\">车位签名：</span><span style=\"border:solid #FFFFFF 1px;width:300px;font-family:'微软雅黑';width:120px; margin-left:74px;font-size:16px;font-weight:bold;\">扪工签名：</span><span style=\"border:solid #FFFFFF 1px;width:300px;font-family:'微软雅黑';width:120px; margin-left:70px;font-size:16px;font-weight:bold;\" >验收签名：</span><span style=\"border:solid #FFFFFF 1px; width:120px; margin-left:70px;font-family:'微软雅黑';font-size:16px;font-weight:bold;\" >打包签名：</span>");

            #region 凯森
            //sb.Append("<div id=\"printdiv\" style=\"width: 720px; \">\n");
            //for (int i = 0; i < lstSum.Count; i++)
            //{
            //    sb.Append("<div style='width: 747px; height: 522px; page-break-after:always'>\n");

            //    sb.Append(" <tr style='height: 800px;'></tr>\n");
            //    sb.Append(" <tr style='height: 110px;'></tr>\n");
            //    sb.Append(" <tr style='height: 65px;'></tr>\n");
            //    sb.Append("<tr style='height: 120px;'><td>" + bch.ReceiverState + bch.ReceiverCity + bch.ReceiverDistrict + "</td></tr>\n");
            //    sb.Append(" <tr style='height: 114px;'></tr>\n");//    sb.Append("<table style='width: 747px; margin-left: 90px; font-size: 15px; font-family:'微软雅黑';border='0' ;cellspacing='0'; cellpadding='0'>\n");
            //    sb.Append("<tr style='height: 80px;'><td style='width: 330px;'>" + bch.ReceiverName + "</td> <td>" + lstSum[i].OuterSkuId + "</td></tr>\n");
            //    sb.Append("<tr style='height: 120px;'><td style='width: 330px;'>" + lstSum[i].OuterIid + "</td> <td>" + lstSum[i].Color + "</td></tr>\n");
            //    sb.Append("</table>\n");
            //    sb.Append("</div>\n");
            //}
            #endregion

            #region 原先标准
            //sb.Append("<table style=\"width: 720px; font-size:12px; font-weight:bold;  padding: 4px;\"  border=\"2\" cellspacing=\"0\" cellpadding=\"0\">\n");
            //sb.Append("<tr><td style=\"width: 90px;\">客户名称</td><td>" + bch.ReceiverName.ToString() + "</td><td>客户ID</td><td colspan=\"2\">" + bch.BuyerNick + "</td><td style=\"width: 70px;\">出货时间</td><td colspan=\"3\">" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</td></tr>\n");
            //sb.Append("<tr><td style=\"width: 90px;\">订单编号</td><td colspan=\"4\">" + bch.Tid + "</td><td>店铺</td><td colspan=\"3\">" + bch.SellerNick + "</td></tr>\n");
            //sb.Append("<tr><td style=\"width: 90px;\">联系电话</td><td colspan=\"4\">" + bch.ReceiverMobile + "</td><td style=\"width: 90px;\">公司名称</td><td colspan=\"3\">" + bch.CorpName + "</td></tr>\n");
            //sb.Append("<tr><td style=\"width: 90px;\">收货地址</td><td colspan=\"4\">" + bch.ReceiverAddress + "</td><td style=\"width: 90px;\">公司地址</td><td colspan=\"3\">" + bch.CorpAddress + "</td></tr>\n");
            //sb.Append("<tr><td style=\"width: 90px;\">物流信息</td><td colspan=\"4\">" + bch.LogisName +" "+bch.ThirdPackType+ " "+bch.LogisMobile+"</td><td style=\"width: 90px;\">公司电话</td><td colspan=\"3\">" + bch.CorpMobile + "</td></tr>\n");
            //sb.Append("<tr><td style=\"width: 90px;\">序号</td><td style=\"width: 100px;\">产品型号</td><td style=\"width: 100px;\">产品名称</td><td style=\"width: 100px;\">产品规格</td><td style=\"width: 100px;\">家具结构</td><td style=\"width: 70px;\">皮布号</td><td style=\"width: 100px;\">数量</td><td style=\"width: 100px;\">包件数</td><td style=\"width: 100px;\">备注</td></tr>\n");
            //int j = 1;
            //string batchcount = ords.Count.ToString();
            //int packcount = 0;
            //for (int i = 0; i < ords.Count; i++)
            //{
            //    sb.Append("<tr><td style=\"width: 90px;\">" + j.ToString() + "</td><td style=\"width: 100px;\">" + ords[i].OuterIid + "</td><td style=\"width: 100px;\">" + ords[i].ItemName + "</td><td style=\"width: 100px;\">" + ords[i].OuterSkuId + "</td><td style=\"width: 100px;\">" + ords[i].Func + "</td><td style=\"width: 70px;\">" + ords[i].ClothNums + "</td><td style=\"width: 100px;\">" + ords[i].Num.ToString() + "</td><td style=\"width: 100px;\">" + ords[i].PackageCount.ToString() + "</td><td style=\"width: 100px;\">" + ords[i].Remark + "</td></tr>\n");
            //    j++;
            //    packcount += ords[i].PackageCount == null ? 0 : int.Parse(ords[i].PackageCount.ToString());
            //}
            //sb.Append("<tr><td style=\"width: 90px;\"></td><td style=\"width: 100px;\"></td><td style=\"width: 100px;\"></td><td style=\"width: 100px;\"></td><td style=\"width: 100px;\"></td><td style=\"width: 70px;\">合计</td><td style=\"width: 100px;\">" + batchcount + "</td><td style=\"width: 100px;\">" + packcount + "</td><td style=\"width: 100px;\"></td></tr>\n");
            //sb.Append("<tr><td style=\"width: 90px;\">客服</td><td style=\"width: 100px;\" colspan=\"2\">" + bch.UserName + "</td><td style=\"width: 100px;\">经办人</td><td style=\"width: 100px;\" colspan=\"3\">" + userName + "</td><td style=\"width: 100px;\">收货人</td><td style=\"width: 100px;\">" + bch.ReceiverName + "</td></tr>\n");
            //sb.Append("<tr><td style=\"width: 90px;\">备注</td><td  colspan=\"8\">" + "1、货物签收时请仔细核对件数、外包装有无明显的破损，若有问题，请及时联系！<br/> 2、收货后，请尽快拆封检查并通风透气，若有问题，请及时拍照并与我们及时联系！" + "</td></tr>\n");
            //sb.Append("</table></div>\n");
            #endregion
            sb.Append("</div>\n");
            return sb.ToString();
        }
    }
}