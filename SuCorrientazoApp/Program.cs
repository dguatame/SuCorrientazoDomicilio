using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SuCorrientazoApp.Models;

namespace SuCorrientazoApp
{
    public class MainClass
    {
        private static readonly string[] _cardinalPoints = { "NORTE", "ESTE", "SUR", "OESTE" };
        private static readonly char[] _allowedOptions = { 'A', 'I', 'D' };
        private static readonly PositionModel _initialPosition = new PositionModel
        {
            X = Convert.ToInt32(ConfigurationManager.AppSettings["PosicionInicialX"]),
            Y = Convert.ToInt32(ConfigurationManager.AppSettings["PosicionInicialY"]),
            CardinalPoint = ConfigurationManager.AppSettings["PuntoCardinalInicial"]
        };


        /// <summary>
        /// Método central donde se ejecuta toda la lógica del negocio.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            List<string> files = Directory.EnumerateFiles("../../AppData/In/", "*.txt").ToList();
            if (!files.Any())
                File.WriteAllText($"../../AppData/Out/error.txt", "No hay archivos en la ruta indicada.");

            Parallel.ForEach(files, (file) => {
                List<string> deliveries = File.ReadAllLines(file).ToList();
                string dronNumber = Regex.Match(file, @"\d+").Value;

                if (!deliveries.Any())
                    throw new Exception($"El archivo del dron {dronNumber} está vacío");

                try
                {
                    CheckDeliveries(deliveries);
                    List<string> resultDeliveries = Deliver(deliveries);
                    File.WriteAllLines($"../../AppData/Out/out{dronNumber}.txt", resultDeliveries);
                }
                catch (Exception ex)
                {
                    File.WriteAllText($"../../AppData/Out/out{dronNumber}.txt", ex.Message);
                }
            });
        }

        /// <summary>
        /// Este método es el encargado de hacer las entregas del dron
        /// </summary>
        /// <param name="deliveries">Listado de entregas</param>
        public static List<string> Deliver(List<string> deliveries)
        {
            if (deliveries == null || !deliveries.Any())
                throw new Exception("El listado de direcciones está vacío");

            List<string> deliveriesResult = new List<string>();
            PositionModel position = new PositionModel
            {
                X = Convert.ToInt32(ConfigurationManager.AppSettings["PosicionInicialX"]),
                Y = Convert.ToInt32(ConfigurationManager.AppSettings["PosicionInicialY"]),
                CardinalPoint = ConfigurationManager.AppSettings["PuntoCardinalInicial"]
            };
            foreach (string delivery in deliveries)
            {
                char[] route = delivery.ToCharArray();
                foreach (char step in route)
                {
                    if (step == 'A')
                        position = GoForward(position);
                    else
                        SetCardinalPoint(step, position);
                }
                deliveriesResult.Add($"({position.X},{position.Y}) dirección {position.CardinalPoint}");
            }
            return deliveriesResult;
        }

        /// <summary>
        /// Este método es el encargado de hacer todas las validaciones de las entregas para saber si la información es valida
        /// de acuerdo a las politicas de la empresa.
        /// </summary>
        /// <param name="deliveries">Listado de entregas</param>
        /// <returns></returns>
        public static void CheckDeliveries(List<string> deliveries)
        {
            PositionModel position = new PositionModel
            {
                X = Convert.ToInt32(ConfigurationManager.AppSettings["PosicionInicialX"]),
                Y = Convert.ToInt32(ConfigurationManager.AppSettings["PosicionInicialY"]),
                CardinalPoint = ConfigurationManager.AppSettings["PuntoCardinalInicial"]
            };

            if (deliveries == null || !deliveries.Any())
                throw new Exception("El listado de direcciones está vacío");

            if (!VerifyNumberOfLunches(deliveries))
                throw new Exception($"Error en ruta, el número de almuerzos excede los {ConfigurationManager.AppSettings["NumeroAlmuerzosMaximo"]} permitidos.");

            if (!VerifyDeliverySteps(deliveries))
                throw new Exception("Error en ruta, letra desconocida.");

            if (!VerifyStreets(deliveries, position))
                throw new Exception($"Error en ruta, las direcciones exceden el número {ConfigurationManager.AppSettings["NumeroCuadrasMaximo"]} de cuadras definidas.");
        }


        /// <summary>
        /// Verifica que el dron no exceda el número máximo de almuerzo configurado en la app
        /// </summary>
        /// <param name="deliveries">Listado de entregas</param>
        /// <returns></returns>
        public static bool VerifyNumberOfLunches(List<string> deliveries)
        {
            if (deliveries == null || !deliveries.Any())
                throw new Exception("El listado de direcciones está vacío");

            int lunches = Convert.ToInt32(ConfigurationManager.AppSettings["NumeroAlmuerzosMaximo"]);
            return deliveries.Count <= lunches;
        }

        /// <summary>
        /// Verifica que las rutas no excedan el número máximo de cuadras a la redonda configuradas en la app
        /// </summary>
        /// <param name="deliveries">Listado de entregas</param>
        /// <param name="position">La posición para empezar la verificación de cuadras</param>
        /// <returns></returns>
        public static bool VerifyStreets(List<string> deliveries, PositionModel position)
        {
            if (deliveries == null || !deliveries.Any())
                throw new Exception("El listado de direcciones está vacío");

            int north = 0;
            int south = 0;
            int west = 0;
            int east = 0;
            int streets = Convert.ToInt32(ConfigurationManager.AppSettings["NumeroCuadrasMaximo"]);
            foreach (string delivery in deliveries)
            {
                char[] route = delivery.ToCharArray();
                foreach (char step in route)
                {
                    if (step == 'A')
                    {
                        switch (position.CardinalPoint)
                        {
                            case "NORTE":
                                north += 1;
                                south -= 1;
                                if (north > streets)
                                {
                                    position = _initialPosition;
                                    return false;
                                }
                                break;
                            case "SUR":
                                south += 1;
                                north -= 1;
                                if (south > streets)
                                {
                                    position = _initialPosition;
                                    return false;
                                }
                                break;
                            case "ESTE":
                                east += 1;
                                west -= 1;
                                if (east > streets)
                                {
                                    position = _initialPosition;
                                    return false;
                                }
                                break;
                            case "OESTE":
                                west += 1;
                                east -= 1;
                                if (west > streets)
                                {
                                    position = new PositionModel();
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        SetCardinalPoint(step, position);
                }
            }
            return true;
        }

        /// <summary>
        /// Verifica que las letras del archivo no sean diferentes a los comandos que puede recibir la app
        /// </summary>
        /// <param name="deliveries">Listado de entregas</param>
        /// <returns></returns>
        public static bool VerifyDeliverySteps(List<string> deliveries)
        {
            if (deliveries == null || !deliveries.Any())
                throw new Exception("El listado de direcciones está vacío");

            foreach (string delivery in deliveries)
            {
                char[] route = delivery.ToCharArray();
                foreach (char step in route)
                {
                    if (!_allowedOptions.Contains(step))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Mantiene actualizada la orientación cardinal del dron, con ellos la app sabe hacia que
        /// punto cardinal esta apuntando el dron
        /// </summary>
        /// <param name="direction">Es la dirección para definir hacia donde queda apuntando el dron</param>
        /// <param name="position">Es la posición actual del dron</param>
        public static void SetCardinalPoint(char direction, PositionModel position)
        {
            if (direction == ' ')
                throw new Exception("El comando viene vacío");

            if (!_allowedOptions.Contains(direction))
                throw new Exception($"{direction} no es un comando válido");

            int index = Array.IndexOf(_cardinalPoints, position.CardinalPoint);
            if (direction == 'I')
                position.CardinalPoint = _cardinalPoints[(index + _cardinalPoints.Length - 1) % _cardinalPoints.Length];
            else if (direction == 'D')
                position.CardinalPoint = _cardinalPoints[(index + 1) % _cardinalPoints.Length];
        }

        /// <summary>
        /// Hace avanzar el dron un espacio hacia adelante tomando la orientación cardinal actual 
        /// </summary>
        /// <param name="position">Es la posición actual del dron</param>
        /// <returns></returns>
        public static PositionModel GoForward(PositionModel position)
        {
            if (position == null)
                throw new Exception("La posición está vacía");

            switch (position.CardinalPoint)
            {
                case "NORTE":
                    position.Y += 1;
                    break;
                case "SUR":
                    position.Y -= 1;
                    break;
                case "ESTE":
                    position.X += 1;
                    break;
                case "OESTE":
                    position.X -= 1;
                    break;
                default:
                    break;
            }
            return position;
        }
    }
}