using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;

namespace dim.rutinas
{
    public sealed class RutinasGenerales
    {
        private const string consignos = "áàäéèëíìïóòöúùuñÁÀÄÉÈËÍÌÏÓÒÖÚÙÜÑçÇ";
        private const string sinsignos = "aaaeeeiiiooouuunAAAEEEIIIOOOUUUNcC";


        #region
        //public void EliminarAcentos(string texto)
        //  {

        //      string s1 ="";

        //      string s2 = "";

        //      s1= "ÁÀÉÈÍÏÓÒÚÜáàèéíïóòúüñç";
        //      s2 = "AAEEIIOOUUaaeeiioouunc";


        //      if(texto.Length != 0)
        //      {
        //          for (int i = 1; i <= s1.Length; i++)
        //          { }


        //      }
        //  }
        #endregion

        // 07.02.2020 FISLA
        public bool PalabrasClaveSQL(string query)
        {
            query = query.ToUpper();

            if (query.Contains("CREATE") || query.Contains("ALTER") || query.Contains("DROP")
                || query.Contains("TRUNCATE") || query.Contains("SELECT") || query.Contains("INSERT")
                || query.Contains("UPDATE") || query.Contains("DELETE")
                )
            {                
                return true;

            }
            else
            {
                return false;
            }

        }


        public long DateDiff(DateInterval intervalo, DateTime dia1, DateTime dia2) //SH-20120402
        {
            long rs = 0;
            TimeSpan diff = dia2.Subtract(dia1);
            switch (intervalo)
            {
                case DateInterval.Day:
                case DateInterval.DayOfYear:
                    rs = (long)diff.TotalDays;
                    break;
                case DateInterval.Hour:
                    rs = (long)diff.TotalHours;
                    break;
                case DateInterval.Minute:
                    rs = (long)diff.TotalMinutes;
                    break;
                case DateInterval.Month:
                    rs = (dia2.Month - dia1.Month) + (12 * DateDiff(DateInterval.Year, dia1, dia2));
                    break;
                case DateInterval.Quarter:
                    rs = (long)Math.Ceiling((double)(DateDiff(DateInterval.Month, dia1, dia2) / 3.0));
                    break;
                case DateInterval.Second:
                    rs = (long)(diff.TotalDays / 7);
                    break;
                case DateInterval.Year:
                    rs = dia2.Year - dia1.Year;
                    break;
            }
            return rs;
        }

        public enum DateInterval //SH-20120402
        {
            Day,
            DayOfYear,
            Hour,
            Minute,
            Month,
            Quarter,
            Second,
            Weekday,
            WeekOfYear,
            Year
        }

        public int Weekday(DateTime date, DayOfWeek startDay)
        {

            int diff;

            DayOfWeek dow = date.DayOfWeek;

            diff = dow - startDay;

            if (diff < 0)
            {

                diff += 7;

            }

            return diff;

        }
        
        public int fix(object o) 
        {
            return (int) Math.Truncate(decimal.Parse(o.ToString()));
        }

        public bool IsDate(Object obj)
        {
            string strDate = obj.ToString();
            try
            {
                DateTime dt = DateTime.Parse(strDate);
                if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public string LimpiaRut(string rut)
        {
            
            rut = rut.Replace(".", "");
            rut = rut.Replace(",", "");
            return rut;
        }

        public string LimpiaRutConGuion(string rut)
        {
            rut = rut.Trim();
            rut = rut.Replace(".", "");
            rut = rut.Replace(",", "");
            rut = rut.Replace("-", "");
            rut = rut.Substring(0, (rut.Length - 1));

            return rut;

        }

        public string FechaJuliana(string fecha)
        {
            return DateTime.Parse(fecha).ToString("yyyyMMdd");
        }

        public string FechaJuliana(DateTime fecha)
        {
            return fecha.ToString("yyyyMMdd");
        }
        
        public static int Asc(string s)
        {
            return Encoding.ASCII.GetBytes(s)[0];
        }

        public static char Chr(int c)
        {
            return Convert.ToChar(c);
        }

        public string LimpiaExcepcion(string mensaje)
        {
            var textoSinAcentos = string.Empty;

            foreach (var caracter in mensaje)
            {
                var indexConAcento = consignos.IndexOf(caracter);
                if (indexConAcento > -1)
                    textoSinAcentos = textoSinAcentos + (sinsignos.Substring(indexConAcento, 1));
                else
                    textoSinAcentos = textoSinAcentos + (caracter);
            }

           // return textoSinAcentos;

            return textoSinAcentos.Replace("'", "").Replace(";", "").Replace("\r\n", "");
        }

        public string FormatoMilesString(string monto)
        {
            return long.Parse(monto).ToString("N0");
        }

        public string FormatoMilesString_double(string monto)
        {
            return double.Parse(monto).ToString("N0");
        }

        public string FormatoMilesInt(int monto)
        {
            return monto.ToString("N0");
        }

        public string FormatoMilesDecimal(decimal monto)
        {
            return monto.ToString("N0");
        }

        public string FormatoMilesDouble(double monto)
        {
            return monto.ToString("N0");
        }

        public string FormatoMilesDouble2Decimales(double monto)
        {
            return monto.ToString("N2");
        }

        public string DevuelveFormatoMoneda(string moneda, double monto)
        {
            String resultado = "";
            switch (moneda)
            {
                //peso        
                case "1":
                    resultado = "N0";
                    break;
                //dolar
                case "2":
                    resultado = "N2";
                    break;
                //UF
                case "3":
                    resultado = "N2";
                    break;
                //4 decimales
                case "4":
                    resultado = "N2";
                    break;

                default:
                    resultado = "N2";
                    break;
            }

            return monto.ToString(resultado);
        }

        public DataSet CargarExcel(string archivo)
        {
            string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + archivo + ";" +
                                       "Extended Properties='Excel 8.0;HDR=YES;'";

            DbProviderFactory factory =
            DbProviderFactories.GetFactory("System.Data.OleDb");

            DbDataAdapter adapter = factory.CreateDataAdapter();

            DbCommand selectCommand = factory.CreateCommand();
            selectCommand.CommandText = "SELECT * FROM [Hoja1$]";

            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;

            selectCommand.Connection = connection;

            adapter.SelectCommand = selectCommand;

            DataSet cities = new DataSet();

            adapter.Fill(cities);
            return cities;
        }

        public bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        public int UltimoDiaDelMes(String fecha)
        {

            System.DateTime NuevaFechaI = DateTime.Parse(fecha);

            System.DateTime NuevaFechaFin; // = NuevaFechaI.AddMonths(1);



            int var_anio = NuevaFechaI.Year; // obtengo el año actual

            int var_mesSiguiente = NuevaFechaI.Month + 1; // obtengo el mes siguiente

            int var_mesActual = NuevaFechaI.Month; // obtengo el mes actual



            if (NuevaFechaI.Month == 12)
            {

                var_anio = NuevaFechaI.Year + 1; // obtengo el año actual

                var_mesSiguiente = 1; // obtengo el mes siguiente

            }

            else

                var_mesSiguiente = NuevaFechaI.Month + 1; // obtengo el mes siguiente



            NuevaFechaI = Convert.ToDateTime("01/" + var_mesActual + "/" + var_anio);// pongo el 1 porque siempre es el primer día obvio

            NuevaFechaFin = Convert.ToDateTime("01/" + var_mesSiguiente + "/" + var_anio).AddDays(-1); //resto un día al mes y con esto obtengo el ultimo día

            return NuevaFechaFin.Day;



        }

        public string Codificacion(string cadena)
        {

            String resultado = cadena;

            resultado = resultado.Replace("&", "&amp;");
            resultado = resultado.Replace("<", "&lt;");
            resultado = resultado.Replace(">", "&gt;");
            resultado = resultado.Replace("“", "&quot;");
            resultado = resultado.Replace("‘", "&apos;");

            return resultado.Replace("\n", "").Replace("\t", "").Trim();

        }

        public string SaltoLinea(string cadena)
        {
            //hasta 76 caracteres

            Encoding encoding = Encoding.GetEncoding("ISO-8859-1");

            String resultado = cadena;
            String salto = "\n";

            int lineas = 65;

            for (int i = 0; i <= cadena.Length; i += lineas)
            {

                try
                {
                    if (i == 0)
                        resultado = salto + resultado.Substring(i, lineas) + salto;
                    else
                    {
                        if (cadena.Substring((i + 1)).Length < lineas)
                            resultado += cadena.Substring((i + 1), cadena.Substring((i + 1)).Length) + salto;
                        else
                            resultado += cadena.Substring((i + 1), lineas) + salto;
                    }

                }

                catch (Exception e)
                {
                    resultado = "";
                }

            }

            return resultado;

        }

        public string ComasXpuntosMontos(string valor)
        {
            
            //solo para montos miles
            return valor.Replace(",", ".");

            //if (valor.IndexOf(char.Parse(",")) >= 0)
            //    return valor.Substring(0, valor.IndexOf(char.Parse(",")));
            //else
            //    return valor;

        }

        public string ComasXpuntosDouble(string valor)
        {

            return double.Parse(valor).ToString("N4").Replace(".", "").Replace(",", ".");

        }
		
		public string ComasXpuntosDouble6(string valor)
        {

            return double.Parse(valor).ToString("N6").Replace(".", "").Replace(",", ".");

        }

        public string ComasXpuntosDecimal(string valor)
        {
            return decimal.Parse(valor).ToString("N2").Replace(".", "").Replace(",", ".");
        }		
		
        //public List<ClsDrop> DevuelveUltimosAños(int cantidadAños) 
        //{
        //    List<ClsDrop> coll = new List<ClsDrop>();
        //    DateTime hoy = DateTime.Now;

        //    for (int i = hoy.Year; i>=(hoy.Year-cantidadAños);i--)
        //    {
        //        ClsDrop drop = new ClsDrop(i.ToString(), i.ToString());
        //        coll.Add(drop);
        //    }

        //    return coll;

        //}

        //public List<ClsDrop> DevuelveMeses()
        //{
        //    List<ClsDrop> coll = new List<ClsDrop>();
            
        //    for (int i=1;i<=12; i++)
        //    {
        //        ClsDrop drop = new ClsDrop(i.ToString(), RetornaMes(i));
        //        coll.Add(drop);
        //    }

        //    return coll;

        //}

        public string  RetornaMes(int Mes)
        {

            switch (Mes)
                {
                case 1:
                    return "ENERO";
                case 2:
                    return "FEBRERO";
                case 3:
                    return "MARZO";
                case 4:
                    return "ABRIL";
                case 5:
                    return "MAYO";
                case 6:
                    return "JUNIO";
                case 7:
                    return "JULIO";
                case 8:
                    return "AGOSTO";
                case 9:
                    return "SEPTIEMBRE";
                case 10:
                    return "OCTUBRE";
                case 11:
                    return "NOVIEMBRE";
                case 12:
                    return "DICIEMBRE";
            }

            return null;

        }

        private bool CheckLeap(int year)
        {
            if ((year % 4 == 0) && (year % 100 != 0) || (year % 400 == 0))
                return true;
            else return false;
        }

        public String BindDays(int year, int month)
        {
            int i;
            ArrayList AlDay = new ArrayList();

            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    for (i = 1; i <= 31; i++)
                        AlDay.Add(i);
                    break;
                case 2:
                    if (CheckLeap(year))
                    {
                        for (i = 1; i <= 29; i++)
                            AlDay.Add(i);
                    }
                    else
                    {
                        for (i = 1; i <= 28; i++)
                            AlDay.Add(i);
                    }
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    for (i = 1; i <= 30; i++)
                        AlDay.Add(i);
                    break;
            }
            //DropDownList3.DataSource = AlDay;
            //DropDownList3.DataBind();

            return AlDay.Count.ToString();
        }

        public String GeneraIdentificador(DateTime FechaNow)
        {
            String Dato = FechaNow.ToString("yyyy-MM-dd HH:mm:ss");

            Dato = Dato.Replace("-", "");
            Dato = Dato.Replace(" ", "");
            Dato = Dato.Replace(":", "");
            Dato = "00" + Dato;
            return Dato;
        }

        public void AbrirPDF(String Path)
        {
            try
            {
                System.Diagnostics.Process.Start(Path);
            }
            catch (Exception)
            { }
        }
       
        public bool ValidarEmail(string email)
        {
            string expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //public byte[] Image2Bytes(Image img)
        //{
        //    string sTemp = Path.GetTempFileName();
        //    FileStream fs = new FileStream(sTemp, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        //    img.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
        //    fs.Position = 0;
        //    //
        //    int imgLength = Convert.ToInt32(fs.Length);
        //    byte[] bytes = new byte[imgLength];
        //    fs.Read(bytes, 0, imgLength);
        //    fs.Close();
        //    return bytes;
        //}

        //public Image Bytes2Image(byte[] bytes)
        //{
        //    if (bytes == null) return null;
        //    //
        //    MemoryStream ms = new MemoryStream(bytes);
        //    Bitmap bm = null;
        //    try
        //    {
        //        bm = new Bitmap(ms);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex.Message);
        //    }
        //    return bm;
        //}

        public String DevuelveCarpeta(string filename)
        {
            string carpeta = "";
            int posicion = 0;

            char[] letras = filename.ToCharArray();

            for (int i = (letras.Count()-1); i >= 0; i--)
            {
                if (letras.GetValue(i).ToString() == "/" | letras.GetValue(i).ToString() == "\\")
                {
                    posicion = i;
                    break;
                }
            }
            
            carpeta = filename.Substring(0, (filename.Length -(filename.Length - posicion)));

            return carpeta;

        }

        public bool ValidaFormato_Fecha(string fecha)
        {
            //valida si el ingreso de fechas es valido
            bool valida = false;
            int pos;
            string[] fech_array;
            pos = fecha.IndexOf("/");
            
            if (pos == 0)
            {
                valida = false;
            }
            else
            {
                fech_array = fecha.Split(new char[] { '/' });
                //fecha español
                if (fech_array[0].Length == 2)
                {
                    if (Convert.ToInt16(fech_array[1].ToString()) >= 13)//meses
                    {
                        valida = false;
                        return valida;
                    }
                    else
                    {
                        if (Convert.ToInt16(fech_array[1].ToString()) == 2)//febrero
                        {
                            if ((Convert.ToInt16(fech_array[2].ToString()) % 4) == 0)//año bisiesto
                            {
                                if (Convert.ToInt16(fech_array[0].ToString()) > 29)//dia febrero
                                {
                                    valida = false;
                                    return valida;
                                }
                            }
                            else
                            {
                                if (Convert.ToInt16(fech_array[0].ToString()) > 28)//dia febrero
                                {
                                    valida = false;
                                    return valida;
                                }
                            }
                        }
                        else
                        {
                            if ((Convert.ToInt16(fech_array[1].ToString()) == 1) |
                                (Convert.ToInt16(fech_array[1].ToString()) == 3) |
                                (Convert.ToInt16(fech_array[1].ToString()) == 5) |
                                (Convert.ToInt16(fech_array[1].ToString()) == 7) |
                                (Convert.ToInt16(fech_array[1].ToString()) == 8) |
                                (Convert.ToInt16(fech_array[1].ToString()) == 10) |
                                (Convert.ToInt16(fech_array[1].ToString()) == 12))//dias mes
                            {
                                if (Convert.ToInt16(fech_array[0].ToString()) > 31 | (Convert.ToInt16(fech_array[0].ToString()) < 1))
                                {
                                    valida = false;
                                    return valida;
                                }
                            }
                            else
                            {
                                if (Convert.ToInt16(fech_array[0].ToString()) > 30 | (Convert.ToInt16(fech_array[0].ToString()) < 1))
                                {
                                    valida = false;
                                    return valida;
                                }
                            }
                        }
                    }
                }
                else//fecha inglesa
                {
                    //falta completar
                }
                valida = true;
            }
            return valida;
        }

        public DateTime FechaJuliana_A_DateTime(string fecha)
        {
            DateTime fechax;
            string anio, mes, dia, fechay; ;

            anio = fecha.Substring(0, 4);
            mes = fecha.Substring(4, 2);
            dia = fecha.Substring(6, 2);

            fechay = dia + "/" + mes + "/" + anio;
            fechax = DateTime.Parse(fechay);
            return fechax;
        }

        public string LetraAcentos(string pChar)
        {
            int i;
            int j;
            string cadena1 = "";
            string cadena2 = "";
            string cadenafinal = "";
            int comienzo = 0;
            for (i = 1; (i <= (pChar.Length - 1)); i++)
            {
                switch (pChar.Substring((i - 1), 1))
                {
                    case "&":
                        cadena1 = pChar.Substring(0, (i - 1));
                        switch (pChar.Substring((i - 1), 6))
                        {
                            case "&#180;":
                                cadena2 = (cadena1 + "´");
                                break;

                            case "&#225;":
                                cadena2 = (cadena1 + "á");
                                break;
                            case "&#233;":
                                cadena2 = (cadena1 + "é");
                                break;
                            case "&#237;":
                                cadena2 = (cadena1 + "í");
                                break;
                            case "&#243;":
                                cadena2 = (cadena1 + "ó");
                                break;
                            case "&#250;":
                                cadena2 = (cadena1 + "ú");
                                break;
                            case "&#252;":
                                cadena2 = (cadena1 + "ü");
                                break;
                            case "&#193;":
                                cadena2 = (cadena1 + "Á");
                                break;
                            case "&#201;":
                                cadena2 = (cadena1 + "É");
                                break;
                            case "&#205;":
                                cadena2 = (cadena1 + "Í");
                                break;
                            case "&#211;":
                                cadena2 = (cadena1 + "Ó");
                                break;
                            case "&#218;":
                                cadena2 = (cadena1 + "Ú");
                                break;
                            case "&#209;":
                                cadena2 = (cadena1 + "Ñ");
                                break;
                            case "&#241;":
                                cadena2 = (cadena1 + "ñ");
                                break;
                            case "&#176;":
                                cadena2 = (cadena1 + "º");
                                break;
                            case "&#186;":
                                cadena2 = (cadena1 + "º");
                                break;
                            case "&#161;":
                                cadena2 = (cadena1 + "¡");
                                break;

                        }
                        j = (((int)(pChar.Length)) - ((int)(cadena1.Length)));
                        comienzo = (((int)(cadena1.Length)) + 7);
                        cadenafinal = pChar.Substring(comienzo - 1);
                        pChar = (cadena2 + cadenafinal);
                        break;
                }
            }
            string LetraAcento = pChar;
            return LetraAcento;
        }

        public String Extraer_Extension(string archivo, string caracter)
        {
            string extension = "";
            if (caracter.Equals(".") & (archivo.IndexOf(caracter) == 0))
                return "";

            if (caracter.Equals("|") & (archivo.IndexOf(caracter) == 0))
                return "";
            int largo = archivo.Length - archivo.LastIndexOf(caracter);
            extension = archivo.Substring(archivo.LastIndexOf(caracter), largo);
            return extension;
        }
        
        public bool IsAñoBisiesto(int YYYY ){

            int calc,calc1,calc2;
            calc = YYYY % 4;
            calc1 = YYYY % 100;
            calc2 = YYYY % 400;

            if ((calc == 0) & ((calc1 != 0 | calc2 == 0)))
            {
                return true;
            }
            return false;

        }

        public string RetornaDiaIngles(int num_dia_semana)
        {
            string dia = "";
                
            switch (num_dia_semana)
            {
                case 1:
                    dia = "Monday";
                    break;
                case 2:
                    dia = "Tuesday";
                    break;
                case 3:
                    dia = "Wednesday";
                    break;
                case 4:
                    dia = "Thursday";
                    break;
                case 5:
                    dia = "Friday";
                    break;
                case 6:
                    dia = "Saturday";
                    break;
                case 7:
                    dia = "Sunday";
                    break;

            }            

            return dia;
        }

        #region "RUT Y DIGITO"

        public string QuitaDigito(string rut)
        {
            rut = rut.Trim();
            rut = rut.Replace("-", "").Substring(0, rut.Length - 2);

            return rut;
        }

        public string digitoVerificador(long rut)
        {
            String mitemp = "";
            String unNit = rut.ToString();
            int micontador = 0;
            long miresiduo = 0;
            long michequeo = 0;
            List<long> miarreglopa = new List<long>(15);

            miarreglopa.Add(3);
            miarreglopa.Add(7);
            miarreglopa.Add(13);
            miarreglopa.Add(17);
            miarreglopa.Add(19);
            miarreglopa.Add(23);
            miarreglopa.Add(29);
            miarreglopa.Add(37);
            miarreglopa.Add(41);
            miarreglopa.Add(43);
            miarreglopa.Add(47);
            miarreglopa.Add(53);
            miarreglopa.Add(59);
            miarreglopa.Add(67);
            miarreglopa.Add(71);

            michequeo = 0;
            miresiduo = 0;

            for (micontador = 0; micontador <= unNit.Length - 1; micontador++)
            {
                mitemp = unNit.Substring((unNit.Length - 1) - micontador, 1).ToString();
                michequeo = michequeo + long.Parse(mitemp) * int.Parse(miarreglopa[micontador].ToString());
            }

            miresiduo = michequeo % 11;

            if (miresiduo <= 1)
                return miresiduo.ToString();
            else
                return Convert.ToString(11 - miresiduo);

        }

        public bool ValidaRut(double rut, string dig)
        {

            String mitemp = "";
            String unNit = rut.ToString();
            int micontador = 0;
            long miresiduo = 0;
            long michequeo = 0;
            List<long> miarreglopa = new List<long>(15);
            string RutDigito = "";

            miarreglopa.Add(3);
            miarreglopa.Add(7);
            miarreglopa.Add(13);
            miarreglopa.Add(17);
            miarreglopa.Add(19);
            miarreglopa.Add(23);
            miarreglopa.Add(29);
            miarreglopa.Add(37);
            miarreglopa.Add(41);
            miarreglopa.Add(43);
            miarreglopa.Add(47);
            miarreglopa.Add(53);
            miarreglopa.Add(59);
            miarreglopa.Add(67);
            miarreglopa.Add(71);

            michequeo = 0;
            miresiduo = 0;

            for (micontador = 0; micontador <= unNit.Length - 1; micontador++)
            {
                mitemp = unNit.Substring((unNit.Length - 1) - micontador, 1).ToString();
                michequeo = michequeo + long.Parse(mitemp) * int.Parse(miarreglopa[micontador].ToString());
            }

            miresiduo = michequeo % 11;

            if (miresiduo <= 1)
                RutDigito = miresiduo.ToString();
            else
                RutDigito = Convert.ToString(11 - miresiduo);

            if (RutDigito == dig)
                return true;
            else
                return false;
        }    

        public string FormatoRut(decimal rut)
        {
            return rut.ToString("N0");
        }

        #endregion

    }     

}
