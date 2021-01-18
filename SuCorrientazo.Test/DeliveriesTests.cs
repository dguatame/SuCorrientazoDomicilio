using System;
using System.Collections.Generic;
using System.Configuration;
using NUnit.Framework;
using SuCorrientazoApp;
using SuCorrientazoApp.Models;

namespace SuCorrientazoDomicilio.Test
{
    [TestFixture()]
    public class DeliveriesTests
    {
        private static readonly PositionModel _position = new PositionModel
        {
            X = Convert.ToInt32(ConfigurationManager.AppSettings["PosicionInicialX"]),
            Y = Convert.ToInt32(ConfigurationManager.AppSettings["PosicionInicialY"]),
            CardinalPoint = ConfigurationManager.AppSettings["PuntoCardinalInicial"]
        };

        [Test()]
        public void ShouldBeFalse_VerifyNumberOfLunches()
        {
            List<string> deliveries = new List<string> { "AAAAAAA", "AAAAALL", "DDAAALL", "AAAAAAA" };
            bool expected = false;
            bool actual = MainClass.VerifyNumberOfLunches(deliveries);
            Assert.AreEqual(expected, actual);
        }

        [Test()]
        public void ShouldBeTrue_VerifyNumberOfLunches()
        {
            List<string> deliveries = new List<string> { "AAAAAAA", "AAAAALL", "DDAAALL" };
            bool expected = true;
            bool actual = MainClass.VerifyNumberOfLunches(deliveries);
            Assert.AreEqual(expected, actual);
        }

        [Test()]
        public void ShouldThrowException_VerifyNumberOfLunches()
        {
            List<string> deliveries = new List<string>();
            var ex = Assert.Throws<Exception>(() => MainClass.VerifyNumberOfLunches(deliveries));
            Assert.That(ex.Message, Is.EqualTo("El listado de direcciones está vacío"));
        }

        [Test()]
        public void ShouldBeFalse_VerifyStreets()
        {
            List<string> deliveries = new List<string> { "AAAAAAAAAAA" };
            bool expected = false;
            bool actual = MainClass.VerifyStreets(deliveries, _position);
            Assert.AreEqual(expected, actual);
        }


        [Test()]
        public void ShouldBeTrue_VerifyStreets()
        {
            List<string> deliveries = new List<string> { "AAAAAA" };
            bool expected = true;
            bool actual = MainClass.VerifyStreets(deliveries, _position);
            Assert.AreEqual(expected, actual);
        }

        [Test()]
        public void ShouldThrowException_VerifyStreets()
        {
            List<string> deliveries = new List<string>();
            var ex = Assert.Throws<Exception>(() => MainClass.VerifyStreets(deliveries, _position));
            Assert.That(ex.Message, Is.EqualTo("El listado de direcciones está vacío"));
        }

        [Test()]
        public void ShouldBeFalse_VerifyDeliverySteps()
        {
            List<string> deliveries = new List<string> { "AAAIDAAZ" };
            bool expected = false;
            bool actual = MainClass.VerifyDeliverySteps(deliveries);
            Assert.AreEqual(expected, actual);
        }


        [Test()]
        public void ShouldBeTrue_VerifyDeliverySteps()
        {
            List<string> deliveries = new List<string> { "ADAIAAIAD" };
            bool expected = true;
            bool actual = MainClass.VerifyDeliverySteps(deliveries);
            Assert.AreEqual(expected, actual);
        }

        [Test()]
        public void ShouldThrowException_VerifyDeliverySteps()
        {
            List<string> deliveries = new List<string>();
            var ex = Assert.Throws<Exception>(() => MainClass.VerifyDeliverySteps(deliveries));
            Assert.That(ex.Message, Is.EqualTo("El listado de direcciones está vacío"));
        }


        [Test()]
        public void ShouldChangeCardinalPoint_SetCardinalPoint()
        {
            PositionModel position = new PositionModel
            {
                X = 0,
                Y = 0,
                CardinalPoint = "NORTE"
            };
            MainClass.SetCardinalPoint('I', position);
            Assert.AreEqual("OESTE", position.CardinalPoint);

            MainClass.SetCardinalPoint('I', position);
            Assert.AreEqual("SUR", position.CardinalPoint);

            MainClass.SetCardinalPoint('I', position);
            Assert.AreEqual("ESTE", position.CardinalPoint);

            MainClass.SetCardinalPoint('I', position);
            Assert.AreEqual("NORTE", position.CardinalPoint);

            MainClass.SetCardinalPoint('D', position);
            Assert.AreEqual("ESTE", position.CardinalPoint);

            MainClass.SetCardinalPoint('D', position);
            Assert.AreEqual("SUR", position.CardinalPoint);

            MainClass.SetCardinalPoint('D', position);
            Assert.AreEqual("OESTE", position.CardinalPoint);

            MainClass.SetCardinalPoint('D', position);
            Assert.AreEqual("NORTE", position.CardinalPoint);
        }

        [Test()]
        public void ShouldNotChangeCardinalPoint_SetCardinalPoint()
        {
            PositionModel position = new PositionModel
            {
                X = 0,
                Y = 0,
                CardinalPoint = "NORTE"
            };
            MainClass.SetCardinalPoint('A', position);
            Assert.AreEqual("NORTE", position.CardinalPoint);
        }

        [Test()]
        public void ShouldNotThrowException_SetCardinalPoint()
        {
            Assert.DoesNotThrow(() => MainClass.SetCardinalPoint('I', _position));
            Assert.DoesNotThrow(() => MainClass.SetCardinalPoint('D', _position));
        }

        [Test()]
        public void ShouldThrowExceptionEmpty_SetCardinalPoint()
        {
            var ex = Assert.Throws<Exception>(() => MainClass.SetCardinalPoint(' ', _position));
            Assert.That(ex.Message, Is.EqualTo("El comando viene vacío"));
        }

        [Test()]
        public void ShouldThrowExceptionInvalid_SetCardinalPoint()
        {
            char direction = 'Z';
            var ex = Assert.Throws<Exception>(() => MainClass.SetCardinalPoint(direction, _position));
            Assert.That(ex.Message, Is.EqualTo($"{direction} no es un comando válido"));
        }

        [Test()]
        public void ShouldThrowException_GoForward()
        {
            PositionModel position = null;
            var ex = Assert.Throws<Exception>(() => MainClass.GoForward(position));
            Assert.That(ex.Message, Is.EqualTo("La posición está vacía"));
        }

        [Test()]
        public void GoForwardNorthOnePosition_GoForward()
        {
            PositionModel position = new PositionModel
            {
                X = 0,
                Y = 0,
                CardinalPoint = "NORTE"
            };
            PositionModel expected = new PositionModel
            {
                X = 0,
                Y = 1,
                CardinalPoint = "NORTE"
            };
            PositionModel actual = MainClass.GoForward(position);
            Assert.AreEqual(expected.Y, actual.Y);
            Assert.AreEqual(expected.X, actual.X);
            Assert.AreEqual(expected.CardinalPoint, actual.CardinalPoint);


            position = new PositionModel
            {
                X = 0,
                Y = 0,
                CardinalPoint = "SUR"
            };
            expected = new PositionModel
            {
                X = 0,
                Y = -1,
                CardinalPoint = "SUR"
            };
            actual = MainClass.GoForward(position);
            Assert.AreEqual(expected.Y, actual.Y);
            Assert.AreEqual(expected.X, actual.X);
            Assert.AreEqual(expected.CardinalPoint, actual.CardinalPoint);

            position = new PositionModel
            {
                X = 0,
                Y = 0,
                CardinalPoint = "OESTE"
            };
            expected = new PositionModel
            {
                X = -1,
                Y = 0,
                CardinalPoint = "OESTE"
            };
            actual = MainClass.GoForward(position);
            Assert.AreEqual(expected.Y, actual.Y);
            Assert.AreEqual(expected.X, actual.X);
            Assert.AreEqual(expected.CardinalPoint, actual.CardinalPoint);

            position = new PositionModel
            {
                X = 0,
                Y = 0,
                CardinalPoint = "ESTE"
            };
            expected = new PositionModel
            {
                X = 1,
                Y = 0,
                CardinalPoint = "ESTE"
            };
            actual = MainClass.GoForward(position);
            Assert.AreEqual(expected.Y, actual.Y);
            Assert.AreEqual(expected.X, actual.X);
            Assert.AreEqual(expected.CardinalPoint, actual.CardinalPoint);
        }


    }
}
