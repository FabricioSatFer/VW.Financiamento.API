using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Financiamento.Application.Helpers
{
    public class CNPJHelper
    {
        public static bool Validar(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            if (cnpj.Length != 14)
                return false;

            var tempCnpj = cnpj.Substring(0, 12);
            if (!int.TryParse(cnpj.Substring(12, 2), out int dvInformado))
            {
                return false;
            }

            var dv1Calculado = CalcularDigitoVerificador(tempCnpj);
            var dv2Calculado = CalcularDigitoVerificador(tempCnpj + dv1Calculado);
            var dvCalculado = dv1Calculado * 10 + dv2Calculado;
            return dvCalculado == dvInformado;
        }

        private static int CalcularDigitoVerificador(string cnpjParcial)
        {
            int[] pesos = cnpjParcial.Length == 12
                ? new int[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 }
                : new int[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            int soma = 0;
            for (int i = 0; i < cnpjParcial.Length; i++)
            {
                var valor = ObterValorParaCalculo(cnpjParcial[i]);
                if (valor == -1)
                    return -1; 

                soma += valor * pesos[i];
            }
            var resto = soma % 11;
            return (resto < 2) ? 0 : 11 - resto;
        }

        private static int ObterValorParaCalculo(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';

            if (c >= 'A' && c <= 'Z')
                return c - '0';

            return -1;
        }

        public static string FormatarMascaraCNPJ(string cnpjSemMascara)
        {
            if (string.IsNullOrWhiteSpace(cnpjSemMascara))
                return string.Empty;

            var cnpjLimpo = Regex.Replace(cnpjSemMascara, @"[./-]", "").ToUpper();
            if (cnpjLimpo.Length != 14)
                return cnpjSemMascara;

            return $"{cnpjLimpo.Substring(0, 2)}.{cnpjLimpo.Substring(2, 3)}.{cnpjLimpo.Substring(5, 3)}/{cnpjLimpo.Substring(8, 4)}-{cnpjLimpo.Substring(12, 2)}";
        }
    }
}
