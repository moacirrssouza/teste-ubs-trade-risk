# UBS Trade Risk Classification API

## Visão Geral

API REST em ASP.NET Core 8 para classificação automática de operações financeiras (trades) de acordo com o nível de risco. O sistema processa milhares de trades em tempo real, atribuindo categorias de risco baseadas em regras de negócio e análise de distribuição de risco de carteira.

## Arquitetura

O projeto segue uma arquitetura em camadas, implementando princípios SOLID e padrões de design:

```
UBS.TradeRisk/
├── UBS.TradeRisk.Domain/          # Camada de Domínio (DDD)
│   ├── Entities/                  # Entidades de negócio
│   ├── Enums/                     # Enumerações
│   ├── Specifications/            # Especificações de negócio
│   └── Repositories/              # Interfaces de repositório
├── UBS.TradeRisk.Application/      # Camada de Aplicação
│   ├── DTOs/                      # Data Transfer Objects
│   ├── Services/                  # Serviços de aplicação
│   └── Extensions/                # Extensões de DI
├── UBS.TradeRisk.Infra/           # Camada de Infraestrutura
│   ├── Data/                      # DbContext e configurações EF Core
│   ├── Repositories/              # Implementação de repositórios
│   └── Extensions/                # Extensões de DI
├── UBS.TradeRisk.Api/             # Camada de Apresentação
│   ├── Controllers/               # Controladores REST
│   ├── Program.cs                 # Configuração da aplicação
│   └── appsettings.json           # Configurações
└── UBS.TradeRisk.Tests/           # Testes
    ├── Domain/                    # Testes de domínio
    └── Application/               # Testes de aplicação
```

## Padrões de Design Implementados

### 1. **Domain-Driven Design (DDD)**
- Entidades de domínio com lógica de negócio encapsulada
- Especificações para regras de classificação
- Value Objects e Aggregates

### 2. **Repository Pattern**
- Interface `ITradeRepository` em Domain
- Implementação com Entity Framework Core na Infra
- Abstração do acesso a dados

### 3. **Service Layer**
- Serviços de aplicação (`ITradeClassificationService`, `IRiskDistributionAnalysisService`)
- Orquestração de regras de negócio
- Separação de responsabilidades

### 4. **Factory Pattern**
- Método factory `Trade.Create()` para criar trades válidas
- Garante criação respeitando invariantes

### 5. **Dependency Injection**
- Configuração centralizada em extensões
- Ciclos de vida apropriados (Scoped, Singleton)
- Loose coupling entre camadas

### 6. **SOLID Principles**
- **Single Responsibility**: Cada classe tem uma responsabilidade
- **Open/Closed**: Fácil extensão sem modificação
- **Liskov Substitution**: Implementações respeitam contratos
- **Interface Segregation**: Interfaces focadas e específicas
- **Dependency Inversion**: Depende de abstrações

## Regras de Classificação de Risco

```
LOWRISK:
  - Valor < 1.000.000

MEDIUMRISK:
  - Valor >= 1.000.000 AND Setor = "Public"

HIGHRISK:
  - Valor >= 1.000.000 AND Setor = "Private"
```

## Endpoints da API

### 1. Classificação de Trades

**POST** `/api/tradesclassification/classify`

Classifica uma lista de trades de acordo com as regras de risco.

**Request:**
```json
[
  {
    "value": 2000000,
    "clientSector": "Private",
    "clientId": "CLI001"
  },
  {
    "value": 400000,
    "clientSector": "Public",
    "clientId": "CLI002"
  },
  {
    "value": 500000,
    "clientSector": "Public",
    "clientId": "CLI003"
  },
  {
    "value": 3000000,
    "clientSector": "Public",
    "clientId": "CLI004"
  }
]
```

**Response (200 OK):**
```json
{
  "categories": ["HIGHRISK", "LOWRISK", "LOWRISK", "MEDIUMRISK"]
}
```

### 2. Análise de Distribuição de Risco

**POST** `/api/riskanalysis/analyze`

Classifica trades e retorna análise estatística da distribuição de risco.

**Request:**
```json
[
  {
    "value": 2000000,
    "clientSector": "Private",
    "clientId": "CLI001"
  },
  {
    "value": 400000,
    "clientSector": "Public",
    "clientId": "CLI002"
  },
  {
    "value": 500000,
    "clientSector": "Public",
    "clientId": "CLI003"
  },
  {
    "value": 3000000,
    "clientSector": "Public",
    "clientId": "CLI004"
  }
]
```

**Response (200 OK):**
```json
{
  "categories": ["HIGHRISK", "LOWRISK", "LOWRISK", "MEDIUMRISK"],
  "summary": {
    "LOWRISK": {
      "count": 2,
      "totalValue": 900000,
      "topClient": "CLI002"
    },
    "MEDIUMRISK": {
      "count": 1,
      "totalValue": 3000000,
      "topClient": "CLI004"
    },
    "HIGHRISK": {
      "count": 1,
      "totalValue": 2000000,
      "topClient": "CLI001"
    }
  },
  "processingTimeMs": 45
}
```

## Configuração e Execução

### Pré-requisitos
- .NET 8 SDK
- SQL Server (LocalDB ou instância remota)
- Visual Studio 2022+ ou VS Code

### Passos de Execução

1. **Restaurar dependências**
   ```powershell
   dotnet restore
   ```

2. **Configurar Connection String**
   
   Editar `UBS.TradeRisk.Api/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=seu-servidor;Database=TradeRiskDb;Trusted_Connection=true;"
     }
   }
   ```

3. **Criar Migration (se necessário)**
   ```powershell
   dotnet ef migrations add InitialCreate --project UBS.TradeRisk.Infra --startup-project UBS.TradeRisk.Api
   ```

4. **Atualizar banco de dados**
   ```powershell
   dotnet ef database update --project UBS.TradeRisk.Infra --startup-project UBS.TradeRisk.Api
   ```

5. **Executar a aplicação**
   ```powershell
   dotnet run --project UBS.TradeRisk.Api
   ```

6. **Acessar Swagger**
   
   Abrir navegador: `https://localhost:7001/swagger`

### Testes

**Executar todos os testes:**
```powershell
dotnet test
```

**Executar testes de um projeto específico:**
```powershell
dotnet test UBS.TradeRisk.Tests
```

**Executar testes de uma classe específica:**
```powershell
dotnet test --filter "ClassName=TradeEntityTests"
```

## Cobertura de Testes

O projeto inclui testes abrangentes cobrindo:

### Testes de Domínio
- ✅ Criação de entidades Trade com validação
- ✅ Classificação de risco
- ✅ Regras de negócio
- ✅ Casos de erro e validação

### Testes de Aplicação
- ✅ Serviço de classificação
- ✅ Serviço de análise de distribuição
- ✅ Validação de entrada
- ✅ Ordenação e integridade de dados
- ✅ Performance com grandes volumes

## Validações e Tratamento de Erro

### Validações de Entrada
- ✅ Valor deve ser maior que zero
- ✅ Setor deve ser "Public" ou "Private"
- ✅ ClientId não pode ser vazio
- ✅ Máximo 100.000 trades por requisição

### Tratamento de Erro
- ✅ HTTP 400 para erros de validação
- ✅ HTTP 500 para erros de servidor
- ✅ Mensagens de erro descritivas
- ✅ Logging de todas as operações

## Performance e Escalabilidade

### Otimizações Implementadas
- **LINQ Otimizado**: Queries eficientes com EF Core
- **Índices de Banco de Dados**: Índices em ClientSector, RiskCategory, ClientId, CreatedAt
- **Precision Decimal**: Precisão de até 18 dígitos para valores monetários
- **Async/Await**: Operações assíncronas em todo o pipeline
- **Precisão Temporal**: Usar `DateTime.UtcNow` para consistência

### Requisitos de Performance
- ✅ Processa até 100.000 trades em uma única requisição
- ✅ Tempo de processamento registrado em milissegundos
- ✅ Cliente com maior exposição identificado por categoria

## Logging

O projeto utiliza **Serilog** para logging estruturado:

- **Console**: Logs em tempo real durante desenvolvimento
- **Arquivo**: Logs em `logs/trade-risk-{data}.txt` para auditoria
- **Níveis**: Information, Warning, Error

Exemplo de logs:
```
[INF] Iniciando classificação de 4 trades
[INF] Classificação completada com sucesso para 4 trades
[INF] Análise completada em 45ms para 4 trades
```

## Decisões Técnicas

### 1. **Arquitetura em Camadas**
- **Justificativa**: Separação clara de responsabilidades, testabilidade e manutenibilidade

### 2. **Entity Framework Core**
- **Justificativa**: ORM maduro, suporte nativo para async, LINQ potente

### 3. **DTOs para Transferência de Dados**
- **Justificativa**: Desacoplamento entre API e lógica interna, maior flexibilidade

### 4. **Especificações para Regras**
- **Justificativa**: Fácil extensão e modificação de regras sem alterar serviços

### 5. **Dependency Injection Nativa**
- **Justificativa**: Built-in no ASP.NET Core, reduz dependências externas

## Licença

Desenvolvido para fins educacionais e técnicos.

## Contato e Suporte

Para dúvidas ou sugestões sobre a implementação, favor consultar a documentação técnica do projeto.
