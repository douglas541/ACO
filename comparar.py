import matplotlib.pyplot as plt

# Função para ler os tempos de execução a partir de um arquivo
def ler_tempos_de_execucao(file_path):
    tempos = {}
    with open(file_path, 'r') as file:
        for line in file:
            partes = line.split(':')
            iteracoes = int(partes[0].split()[-2])
            tempo = float(partes[1].split()[0])
            tempos[iteracoes] = tempo
    return tempos

# Ler os tempos de execução dos arquivos
tempos_paralelo = ler_tempos_de_execucao('tempo_execucao_paralelo.txt')
tempos_sequencial = ler_tempos_de_execucao('tempo_execucao_sequencial.txt')

# Plotar os tempos de execução
iteracoes = sorted(tempos_paralelo.keys())

tempos_paralelo_valores = [tempos_paralelo[i] for i in iteracoes]
tempos_sequencial_valores = [tempos_sequencial[i] for i in iteracoes]

plt.figure(figsize=(10, 6))
plt.plot(iteracoes, tempos_paralelo_valores, label='Paralelo', marker='o')
plt.plot(iteracoes, tempos_sequencial_valores, label='Sequencial', marker='o')
plt.xlabel('Número de Iterações')
plt.ylabel('Tempo de Execução (segundos)')
plt.title('Comparação do Tempo de Execução: Algoritmo Paralelo vs Sequencial')
plt.legend()
plt.grid(True)
plt.savefig('comparacao_tempo_execucao.png')
plt.show()

# Calcular o speedup
speedup = [tempos_sequencial[i] / tempos_paralelo[i] for i in iteracoes]

# Plotar o gráfico de speedup
plt.figure(figsize=(10, 6))
plt.plot(iteracoes, speedup, label='Speedup', marker='o')
plt.xlabel('Número de Iterações')
plt.ylabel('Speedup')
plt.title('Speedup do Algoritmo Paralelo em Comparação ao Sequencial')
plt.legend()
plt.grid(True)
plt.savefig('speedup.png')
plt.show()
