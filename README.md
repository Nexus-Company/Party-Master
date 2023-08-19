# Party Master 
O projeto "Party Master" tem como ideia a participação ativa dos integrantes de um ambiente nas músicas tocadas, a ideia é fazer com que as pessoas possam 
interagir igualitariamente de forma organizada, dessa forma tornando o ambiente mais divertido.

### Música infinita 
Uma festa sem música não é a mesma coisa, não é mesmo ? Exatamente por isso o projeto inclui um módulo especificado para dar continudade as músicas com base 
nas que foram tocadas anteriormente. Dessa forma sua festa nunca ficará sem música, além disso sempre terá uma para dar continuidade com a mesma vibe.

### Votação para pular música 
Como especificado anteriormente uma festa sem música não é a mesma coisa, porém uma música no momento errado pode estragar a vibe do ambiente. Exatamente por isso será possível 
iniciar uma votação para pular a música atual. Assim que a votação atingir uma porcentagem do público conectado automáticamente a música será pulada. 

> A porcentagem exata de público mínimo necessário para que uma faixa possa ser pulada deverá ser especificada anteriormente. Será necessário também definir o numéro minimo de pessoas, para iniciar o modo de interação.

### Inclusão de música por participantes 
Para torna o público parte do ambiente todos os participantes poderam incluir suas músicas desejadas na filha de reprodução, além também de poder votar para remover uma música da fila de reprodução. 

> Antes de um música ser adicionada a fila de reprodução ele será processada pela API de categorização para saber se o genêro dela está de acordo com os atuais.

> O sistema pode ser configurado para definir um limite de músicas adicionadas por uma única pessoa em uma janela de 45 minutor

## Distribuição de projetos 

# [Manager](Manager/)
Neste módulo do projeto está as partes referente ao gerenciamento ativo das músicas, tendo então partes como [API de gestão](Manager/Nexus.Party.Master.Api), além também das classes de contexto do [Banco de dados](Manager/Nexus.Party.Master.Dal) e por fim o projeto de [Dominio](Manager/Nexus.Party.Master.Doomain) cotnendo a lógica principal do sistema. 

# [Categorizer](Categorizer/)
O módulo de categorização contém os projetos responsáveis pelo ciclo de classificação das faixas.

# [Client](Client/)
