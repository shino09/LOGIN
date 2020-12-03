using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Login.Models;
using System.Web.Security;
using System.Web.Helpers;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Login.Controllers
{
    public class InicioController : Controller
    {
       

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel loginDataModel)
        {
            /**********SE INICIA EL WS_Acceso Y SE DECLARA SUS TIPO DE DATOS PARA ENVIAR*********/
            WS_Acceso.WS_Acceso wa = new WS_Acceso.WS_Acceso();
            wa.Timeout = -1;
            WS_Acceso.CABECERA_Type CABECERA = new WS_Acceso.CABECERA_Type();
            WS_Acceso.DATOS_Type DATOS = new WS_Acceso.DATOS_Type();
            CABECERA.APP_CONSUMIDORA = "Login";
            string[] separar = null;
            string dominio = "";
            string passwordEncriptado = "";

            /******SE INICIA EL WS_Acceso Y SE DECLARA SUS TIPO DE DATOS PARA ENVIAR Y RECIBIR*******/
            WS_Acceso.MENSAJERES_Type MENSAJERES = new WS_Acceso.MENSAJERES_Type();
            string ruc = "";
            string codigoParticipante = "";
            string urlEndPoint = "";
            string semilla = "";

            try
            {

                if (ModelState.IsValid)
                {

                    try
                    {

                        /***********************SE ENCRIPTA LA PASSWORD CON SEMILLA********************/
                        semilla = generarKey();
                        if (semilla != "")
                        {
                            passwordEncriptado = Encriptar(semilla, loginDataModel.Password);
                        }
                        else
                        {
                            ViewBag.error = "Password no valido";
                            return View("Login");
                        }

                        separar = loginDataModel.Email.Split('@');
                            dominio = separar[1];
                            DATOS.Dominio = dominio;
                            MENSAJERES = wa.BuscarCliente(CABECERA, DATOS);
                        

                    }
                    catch (Exception ex)
                    {
                        ViewBag.error = "Error de Conexion";
                        return View("Login");
                    }
                    /********ESTA EL CLIENTE EN LA BASE DE DATOS*******/
                    if (MENSAJERES.INTEGRES.DETALLE.DATOS.Cliente_Ruc != "" && MENSAJERES.INTEGRES.DETALLE.DATOS.Cliente_Ruc != null && MENSAJERES.INTEGRES.DETALLE.DATOS.Cliente_Cod_Part != "" && MENSAJERES.INTEGRES.DETALLE.DATOS.Cliente_Cod_Part != null && MENSAJERES.INTEGRES.DETALLE.DATOS.EndPoint_Cli_Cone_xion != "" && MENSAJERES.INTEGRES.DETALLE.DATOS.Cliente_Cod_Part != "" && MENSAJERES.INTEGRES.DETALLE.DATOS.Cliente_Cod_Part != null && MENSAJERES.INTEGRES.DETALLE.DATOS.EndPoint_Cli_Cone_xion != null)
                    {
                        ruc = MENSAJERES.INTEGRES.DETALLE.DATOS.Cliente_Ruc;
                        codigoParticipante = MENSAJERES.INTEGRES.DETALLE.DATOS.Cliente_Cod_Part;
                        urlEndPoint = MENSAJERES.INTEGRES.DETALLE.DATOS.EndPoint_Cli_Cone_xion;
                    }
                    else
                    {
                        ViewBag.error = "Cliente no Encontrado";
                        return View("Login");
                    }


                    /***********************WS_PRODUCTO********************/
                    if (passwordEncriptado != "")
                    {
                        
                            WS_Producto_Dimension.WS_Producto wp = new WS_Producto_Dimension.WS_Producto();
                            wp.Timeout = -1;
                            wp.Url = urlEndPoint;
                            WS_Producto_Dimension.CABECERA_Type CABECERA_Producto = new WS_Producto_Dimension.CABECERA_Type();
                            WS_Producto_Dimension.DATOS_Type DATOS_Producto = new WS_Producto_Dimension.DATOS_Type();

                            /******SE INICIA EL WS_Acceso Y SE DECLARA SUS TIPO DE DATOS PARA ENVIAR Y RECIBIR******/
                            WS_Producto_Dimension.MENSAJERES_Type MENSAJERES_Producto = new WS_Producto_Dimension.MENSAJERES_Type();

                            try
                            {
                                CABECERA_Producto.APP_CONSUMIDORA = "Login";
                                DATOS_Producto.Usuario = loginDataModel.Email;
                                DATOS_Producto.Password = passwordEncriptado;
                                DATOS_Producto.Key = semilla;
                                MENSAJERES_Producto = wp.SolicitarAcceso(CABECERA_Producto, DATOS_Producto);
                            }
                            catch (Exception ex)
                            {
                                ViewBag.error = "Error de Conexion";
                                return View("Login");
                            }

                            /********ESTAN CORRECTAS LAS CREDENCIALES*******/

                            if (MENSAJERES_Producto.INTEGRES.DETALLE.DATOS.resultCode == "1")
                            {
                                Session["username"] = loginDataModel.Email;
                                ViewBag.ruc = ruc;
                                ViewBag.codigoParticipante = codigoParticipante;
                                ViewBag.email = loginDataModel.Email;


                                return View("Index");
                            }
                            else
                            {
                                ViewBag.error = "Usuario o contraseña incorrectos";
                                return View("Login");
                            }
                        
                     
                    }
                    else
                    {
                        ViewBag.error = "Password no valido";
                        return View("Login");
                    }

                    }
                /*********DATOS MAL ESCRITOS, EMAIL O PASSWORD ***********/
                else
                {

                    return View("Login");

                }
            }
            catch (Exception ex)
            {
                ViewBag.error = "Error";
                return View("Login");
            }

            ViewBag.error = "Usuario o contraseña incorrectos";
            return View("Login");

        }

        /******SALIR O CERRAR SESSION*******/
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Remove("username");
            return RedirectToAction("Login");
        }




        /**GENERA UNA KEY DE 32 CARACTERES**/
        public string generarKey()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[32];
            try
            {
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                string finalString = new String(stringChars);
                return finalString;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /**ENCRIPTA LA PASSWORD PASANDOLE UNA KEY O SEMILLA**/
        public static string Encriptar(string key, string palabra)
        {
            byte[] iv = new byte[16];
            byte[] array;

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                            {
                                streamWriter.Write(palabra);
                            }

                            array = memoryStream.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }

            return Convert.ToBase64String(array);
        }




        /**DESENCRIPTA LA PASSWORD PASANDOLE UNA KEY O SEMILLA, ESTA FUNCION ES LA QUE USA EL WS_Producto**/
        public static string Desencriptar(string key, string palabraEncriptada)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(palabraEncriptada);

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

    }
}