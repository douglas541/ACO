using System.Globalization;

Random random = new Random(42);

// Parâmetros
var alpha = 1.0;  // Importância do feromônio
var beta = 2.0;   // Importância heurística
var rho = 0.5;    // Taxa de evaporação
var numFormigas = 16;
var numCidades = 25;

// Inicialização aleatória de distâncias e feromônios
var distancias = new double[numCidades, numCidades];
var feromonios = new double[numCidades, numCidades];
var heuristicas = new double[numCidades, numCidades];

InitializarMatrizes(distancias, feromonios, heuristicas, numCidades);

// Função para construir solução
Func<double[,], double[,], int, int[]> construirSolucao = (feromonios, heuristicas, numCidades) =>
{
    var tour = new int[numCidades];
    var visitadas = new bool[numCidades];
    var cidadeInicial = random.Next(numCidades);
    tour[0] = cidadeInicial;
    visitadas[cidadeInicial] = true;

    for (var i = 1; i < numCidades; i++)
    {
        var cidadeAtual = tour[i - 1];
        var probabilidades = new double[numCidades];

        for (var j = 0; j < numCidades; j++)
        {
            if (!visitadas[j])
            {
                probabilidades[j] = Math.Pow(feromonios[cidadeAtual, j], alpha) * Math.Pow(heuristicas[cidadeAtual, j], beta);
            }
        }

        var somaProbabilidades = probabilidades.Sum();

        // Verificação para evitar divisão por zero
        if (somaProbabilidades == 0)
        {
            for (var j = 0; j < numCidades; j++)
            {
                if (!visitadas[j])
                {
                    probabilidades[j] = 1;
                }
            }
            somaProbabilidades = probabilidades.Sum();
        }

        for (var j = 0; j < numCidades; j++)
        {
            probabilidades[j] /= somaProbabilidades;
        }

        // Verificação para garantir que as probabilidades são válidas
        if (probabilidades.Any(double.IsNaN))
        {
            throw new Exception("Probabilidades contém NaN");
        }

        var r = random.NextDouble();
        var acumulada = 0.0;
        var proximaCidade = -1;
        for (var j = 0; j < numCidades; j++)
        {
            acumulada += probabilidades[j];
            if (r <= acumulada)
            {
                proximaCidade = j;
                break;
            }
        }

        tour[i] = proximaCidade;
        visitadas[proximaCidade] = true;
    }

    return tour;
};

// Função para atualizar feromônios
Func<int[][], double[,], double[,], double, double[,]> atualizarFeromonios = (tours, distancias, feromonios, rho) =>
{
    var deltaFeromonios = new double[numCidades, numCidades];
    foreach (var tour in tours)
    {
        for (var i = 0; i < tour.Length - 1; i++)
        {
            deltaFeromonios[tour[i], tour[i + 1]] += 1.0 / distancias[tour[i], tour[i + 1]];
        }
        deltaFeromonios[tour[^1], tour[0]] += 1.0 / distancias[tour[^1], tour[0]];
    }

    for (var i = 0; i < numCidades; i++)
    {
        for (var j = 0; j < numCidades; j++)
        {
            feromonios[i, j] = (1 - rho) * feromonios[i, j] + deltaFeromonios[i, j];
        }
    }

    return feromonios;
};

// Função ACO
Action<double[,], double[,], double[,], int, int, int> acoThread = (distancias, feromonios, heuristicas, numCidades, numFormigas, numIteracoes) =>
{
    for (var iteracao = 0; iteracao < numIteracoes; iteracao++)
    {
        var tours = new int[numFormigas][];
        Parallel.For(0, numFormigas, i =>
        {
            tours[i] = construirSolucao(feromonios, heuristicas, numCidades);
        });

        feromonios = atualizarFeromonios(tours, distancias, feromonios, rho);
        Console.WriteLine($"Iteração {iteracao + 1}/{numIteracoes} concluída.");
    }
};

// Função para medir tempo de execução
Func<Action<double[,], double[,], double[,], int, int, int>, double[,], double[,], double[,], int, int, int, (double[,], double)> medirTempoExecucao = (func, distancias, feromonios, heuristicas, numCidades, numFormigas, numIteracoes) =>
{
    var startTime = DateTime.Now;
    func(distancias, feromonios, heuristicas, numCidades, numFormigas, numIteracoes);
    var endTime = DateTime.Now;
    var tempoExecucao = (endTime - startTime).TotalSeconds;
    return (feromonios, tempoExecucao);
};

var iteracoesArray = new int[] { 1000, 5000, 10000, 50000, 100000 };
using var file = new StreamWriter("C:\\Users\\dougl\\source\\repos\\ACO\\tempo_execucao_paralelo.txt");

foreach (var numIteracoes in iteracoesArray)
{
    var (result, tempoExecucao) = medirTempoExecucao(acoThread, distancias, feromonios, heuristicas, numCidades, numFormigas, numIteracoes);
    file.WriteLine($"Tempo de execução para {numIteracoes} iterações: {tempoExecucao.ToString("F6", CultureInfo.InvariantCulture)} segundos");
    Console.WriteLine($"Tempo de execução para {numIteracoes} iterações: {tempoExecucao.ToString("F6", CultureInfo.InvariantCulture)} segundos");
}

void InitializarMatrizes(double[,] distancias, double[,] feromonios, double[,] heuristicas, int numCidades)
{
    for (var i = 0; i < numCidades; i++)
    {
        for (var j = 0; j < numCidades; j++)
        {
            if (i != j)
            {
                distancias[i, j] = random.NextDouble();
                feromonios[i, j] = 1.0;
                heuristicas[i, j] = 1.0 / distancias[i, j];
            }
            else
            {
                distancias[i, j] = 0.0;
                feromonios[i, j] = 1.0;
                heuristicas[i, j] = 0.0;
            }
        }
    }
}
