using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ECC.API_Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages;
using ECC.EntidadeUsuario;


namespace ECC.API_Web.InfraWeb
{
    public class Util
    {
        public static string RemoverAcentos(string texto)
        {

            string s = texto.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < s.Length; k++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(s[k]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(s[k]);
                }
            }
            return sb.ToString();
        }

        public static string MontaStrMenu(int menuPaiId, List<MenuViewModel> lmenu)
        {
            string strMenu = "";
            MenuViewModel menu = lmenu.Where(m => m.Id == menuPaiId).FirstOrDefault();
            if (menu.MenuPaiId != null && menu.MenuPaiId > 0)
                strMenu = MontaStrMenu(menu.MenuPaiId, lmenu);

            strMenu += strMenu == "" ? menu.DescMenu : " > " + menu.DescMenu;
            return strMenu;
        }

        public static IEnumerable<PermissaoGrupoViewModel> PermissaoIdPai(int MenuPaiId, IEnumerable<PermissaoGrupo> lstPermissaoGrupo, IEnumerable<MenuViewModel> lstMenu)
        {
            List<PermissaoGrupoViewModel> lstPermissaoGrupoVM = new List<PermissaoGrupoViewModel>();
            var grupoPermissao = lstPermissaoGrupo.Where(x => x.MenuId == MenuPaiId);
            PermissaoGrupoViewModel permissaoGrupoVM = null;
            MenuViewModel mn = null;

            if (!grupoPermissao.Any())
            {
                permissaoGrupoVM = new PermissaoGrupoViewModel();
                permissaoGrupoVM.MenuId = MenuPaiId;
                lstPermissaoGrupoVM.Add(permissaoGrupoVM);

                mn = lstMenu.Single(x => x.Id == MenuPaiId);
                if (mn.MenuPaiId != null && mn.MenuPaiId > 0)
                    lstPermissaoGrupoVM.AddRange(PermissaoIdPai(mn.MenuPaiId, lstPermissaoGrupo, lstMenu));
            }

            return lstPermissaoGrupoVM;
        }

    }

    public class StringValueAttribute : Attribute
    {
        public string StringValue { get; protected set; }

        public StringValueAttribute(string value)
        {
            this.StringValue = value;
        }
    }
    
    //Remove os acentos da pasta subCategoria

    public static class ObjectExtensions
    {
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            StringValueAttribute[] attrib = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
            return attrib.Length > 0 ? attrib[0].StringValue : null;
        }

        public static DateTime? ToDate(this object value)
        {
            var dtRetorno = DateTime.MinValue;
            var valueString = value.ToString();
            if (DateTime.TryParse(valueString.Length == 8 ? (valueString.Substring(6, 2) + "/" + valueString.Substring(4, 2) + "/" + valueString.Substring(0, 4)) : valueString, out dtRetorno))
                return dtRetorno;
            else return null;
        }

        public static DateTime AddHoraIni(this DateTime Value)
        {

            return Value.AddHours(00).AddMinutes(00).AddSeconds(00);
        }

        public static DateTime AddHoraFim(this DateTime Value)
        {

            return Value.AddHours(23).AddMinutes(59).AddSeconds(59);
        }
        
        public static string ToOracleString(this string Value)
        {
            if (String.IsNullOrEmpty(Value))
            {
                return "NULL";
            }
            else
            {
                return "'" + Value.Replace("'", "''") + "'";
            }
        }

        public static string ToOracleString(this char Value)
        {
            if (Char.IsControl(Value) || Char.IsWhiteSpace(Value))
            {
                return "NULL";
            }
            else
            {

                return "'" + Value + "'";
            }
        }

        public static string ToOracleNumber(this decimal Value)
        {
            return Value.ToString().Replace(",", ".");
        }

        public static string ToOracleNumber(this float Value)
        {
            return Value.ToString().Replace(",", ".");
        }

        public static string ToOracleNumber(this double Value)
        {
            return Value.ToString().Replace(",", ".");
        }

        public static string ToOracleDate(this DateTime? Value)
        {
            if (Value == null)
            {
                return "NULL";
            }
            else
            {
                return "TO_DaTE('" + Value.Value.ToString("dd/MM/yyyy") + "','DD/MM/YYYY') ";
            }
        }

        public static string ToOracleDate(this DateTime Value)
        {

            return "TO_DATE('" + Value.ToString("dd/MM/yyyy") + "','DD/MM/YYYY')";
        }

        public static int TryParseInt(this object Value)
        {
            int result = 0;
            int.TryParse(Value.ToString(), out result);
            return result;
        }

        public static short TryParseShort(this object Value)
        {
            short result = 0;
            short.TryParse(Value.ToString(), out result);
            return result;
        }

        public static ushort TryParseUShort(this object value)
        {
            ushort result = 0;
            ushort.TryParse(value.ToString(), out result);
            return result;
        }
        public static byte TryParseByte(this object Value)
        {
            byte result = 0;
            byte.TryParse(Value.ToString(), out result);
            return result;
        }

        public static long TryParseLong(this object Value)
        {
            long result = 0;
            if (Value != null)
                long.TryParse(Value.ToString(), out result);
            return result;
        }

        public static float TryParseFloat(this object Value)
        {
            float result = 0.0f;
            float.TryParse(Value.ToString(), out result);
            return result;
        }

        public static double TryParseDouble(this object Value)
        {
            double result = 0.0;
            double.TryParse(Value.ToString(), out result);
            return result;
        }

        public static DateTime? TryParseDate(this string Value)
        {
            DateTime result = DateTime.MinValue;
            DateTime.TryParse(Value, out result);
            return result != DateTime.MinValue ? result : new Nullable<DateTime>();
        }
        public static decimal TryParseDecimal(this string Value)
        {
            decimal result = 0;
            decimal.TryParse(Value, out result);
            return result;
        }

        public static char TryParseChar(this object Value)
        {

            char result = ' ';
            char.TryParse(Value.ToString().Substring(0, 1), out result);

            return result;
        }

        public static bool StringValidate(this string Value, string pattern)
        {

            return Regex.IsMatch(Value, pattern);
        }

        public static double RoundDown(this double value, int digits)
        {
            long factor = (long)Math.Pow(10, digits);

            return Math.Truncate(value * factor) / factor;
        }

        public static float RoundDown(this float value, int digits)
        {
            long factor = (long)Math.Pow(10, digits);

            string o = Convert.ToString(value * factor);

            return (float)Math.Truncate(float.Parse(o)) / factor;
        }
    }
}