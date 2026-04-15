using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financiamento.Application.Helpers
{
    public class CPFHelper
    {
        public static bool Validar(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            var cpfLimpo = new string(cpf.Where(char.IsDigit).ToArray());
            if (cpfLimpo.Length != 11)
                return false;

            var tempCpf = cpfLimpo.Substring(0, 9);
            var dv1Calculado = CalcularDigitoVerificador(tempCpf);
            var dv2Calculado = CalcularDigitoVerificador(tempCpf + dv1Calculado);
            var dvCalculado = dv1Calculado * 10 + dv2Calculado;
            return dvCalculado == int.Parse(cpfLimpo.Substring(9, 2));
        }

        private static int CalcularDigitoVerificador(string cpfParcial)
        {
            int peso = cpfParcial.Length + 1;
            int soma = 0;
            foreach (var c in cpfParcial)
            {
                if (!char.IsDigit(c))
                    return -1; 
                soma += (c - '0') * peso--;
            }
            var resto = soma % 11;
            return (resto < 2) ? 0 : 11 - resto;
        }

        public static string FormatarMascaraCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return string.Empty;

            var cpfLimpo = new string(cpf.Where(char.IsDigit).ToArray());
            if (cpfLimpo.Length != 11)
                return cpf; 

            return $"{cpfLimpo.Substring(0, 3)}.{cpfLimpo.Substring(3, 3)}.{cpfLimpo.Substring(6, 3)}-{cpfLimpo.Substring(9, 2)}";
        }
    }
}
