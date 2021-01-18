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

    }
}
