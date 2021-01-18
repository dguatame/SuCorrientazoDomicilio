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
    class MainClass
    {
        private static readonly string[] _cardinalPoints = { "NORTE", "ESTE", "SUR", "OESTE" };
        private static readonly char[] _allowedOptions = { 'A', 'I', 'D' };
        private static readonly PositionModel _initialPosition = new PositionModel
        {
            X = Convert.ToInt32(ConfigurationManager.AppSettings["PosicionInicialX"]),
            Y = Convert.ToInt32(ConfigurationManager.AppSettings["PosicionInicialY"]),
            CardinalPoint = ConfigurationManager.AppSettings["PuntoCardinalInicial"]
        };

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
                    //todo agregar el método para las entregas
                }
                catch (Exception ex)
                {
                    File.WriteAllText($"../../AppData/Out/out{dronNumber}.txt", ex.Message);
                }
            });
        }

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

        public static bool VerifyNumberOfLunches(List<string> deliveries)
        {
            if (deliveries == null || !deliveries.Any())
                throw new Exception("El listado de direcciones está vacío");

            int lunches = Convert.ToInt32(ConfigurationManager.AppSettings["NumeroAlmuerzosMaximo"]);
            return deliveries.Count <= lunches;
        }

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
    }
}
