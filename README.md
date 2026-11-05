# MicroTickets - Sistema de Gestão de Eventos e Ingressos

> **Disciplina:** Engenharia de Software - Arquitetura de Software  
> **Professor:** Eduardo Cizeski Meneghel  
> **Instituição:** Centro Universitário SATC (UNISATC)  
> **Tecnologias:** .NET 8 (C#), SQLite, ASP.NET Core Minimal APIs / Web API, REST, Docker, Render  

---

## 1. Documento de Requisitos

### 1.1. Propósito do Sistema
O **TicketPass** é uma solução de back-end baseada em arquitetura de microsserviços voltada para a gestão de eventos, reserva e emissão de ingressos, cadastro de clientes e processamento de pagamentos simulados. O sistema permite cadastrar eventos com lotação controlada, associar compradores aos ingressos emitidos e gerir o ciclo de vida da transação financeira, garantindo consistência e isolamento entre os domínios.

### 1.2. Usuários do Sistema
* **Organizador do Evento:** Cadastra eventos, define locais, datas e capacidade máxima de participantes.
* **Cliente / Comprador:** Consulta eventos disponíveis, solicita a reserva/emissão de ingressos e acompanha o status de pagamento de seus pedidos.
* **Administrador / Sistema Financeiro:** Consulta relatórios de transações efetuadas e status de liquidação de pagamentos.

### 1.3. Requisitos Funcionais (RF)
* **RF01 - Cadastro de Eventos:** O sistema deve permitir o cadastro, listagem e consulta detalhada de eventos (nome, data, local e capacidade máxima).
* **RF02 - Cadastro de Clientes:** O sistema deve permitir cadastrar e consultar dados de compradores (nome, e-mail e documento).
* **RF03 - Emissão de Ingressos:** O sistema deve permitir a emissão de ingressos com status inicial `Pendente`, vinculados a um evento e a um cliente.
* **RF04 - Validação de Evento na Emissão:** Ao emitir um ingresso, o sistema deve validar se o evento existe e está ativo.
* **RF05 - Validação de Cliente na Emissão:** Ao emitir um ingresso, o sistema deve validar se os dados cadastrais do comprador existem e estão regulares.
* **RF06 - Processamento de Pagamento (Mock):** O sistema deve registrar uma intenção de pagamento para um ingresso emitido e simular sua aprovação imediata.
* **RF07 - Confirmação Automática de Ingresso:** Ao aprovar um pagamento, o sistema deve atualizar de forma automática e imediata o status do ingresso correspondente de `Pendente` para `Confirmado`.

---

## 2. Descritivo Técnico da Arquitetura

O sistema é dividido em **4 microsserviços independentes**, cada um com sua própria base de dados SQLite local, expondo contratos REST documentados via Swagger:

```text
[ Pagamentos-Service ] ───────► (Alteração: PATCH /api/ingressos/{id}/confirmar) ──► [ Ingressos-Service ]
                                                                                           │          │
   ┌────────────────────── (Busca 1: GET /api/eventos/{id}) ───────────────────────────────┘          │
   ▼                                                                                                   ▼
[ Eventos-Service ]                                                                           [ Clientes-Service ]
                                                            (Busca 2: GET /api/clientes/{id}) ─────────┘
```

---

### 2.1. Microsserviços e Responsabilidades

#### 1. `Eventos-Service`
* **Responsabilidade:** Gerir o ciclo de vida dos eventos (criação, capacidade de público, ativação/desativação).
* **Base de Dados:** SQLite (`eventos.db`)
* **Entidade Principal:**
  ```csharp
  public class Evento
  {
      public int Id { get; set; }
      public string Nome { get; set; } = string.Empty;
      public string Local { get; set; } = string.Empty;
      public DateTime Data { get; set; }
      public int CapacidadeMaxima { get; set; }
      public bool Ativo { get; set; } = true;
  }
  ```
* **Endpoints:**
  * `GET /api/eventos` - Lista todos os eventos
  * `GET /api/eventos/{id}` - Busca evento por ID
  * `POST /api/eventos` - Cadastra um novo evento

#### 2. `Clientes-Service`
* **Responsabilidade:** Gestão cadastral de participantes e compradores.
* **Base de Dados:** SQLite (`clientes.db`)
* **Entidade Principal:**
  ```csharp
  public class Cliente
  {
      public int Id { get; set; }
      public string Nome { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public string Documento { get; set; } = string.Empty;
  }
  ```
* **Endpoints:**
  * `GET /api/clientes` - Lista todos os clientes
  * `GET /api/clientes/{id}` - Obtém os dados cadastrais do cliente
  * `POST /api/clientes` - Cadastra um novo cliente

#### 3. `Ingressos-Service`
* **Responsabilidade:** Emissão, controle de tickets e ciclo de vida dos ingressos.
* **Base de Dados:** SQLite (`ingressos.db`)
* **Entidade Principal:**
  ```csharp
  public class Ingresso
  {
      public int Id { get; set; }
      public string CodigoTicket { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
      public int EventoId { get; set; }
      public int ClienteId { get; set; }
      public decimal Preco { get; set; }
      public string Status { get; set; } = "Pendente"; // Pendente, Confirmado, Cancelado
      public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
  }
  ```
* **Endpoints:**
  * `GET /api/ingressos/{id}` - Consulta dados de um ingresso por ID
  * `POST /api/ingressos` - Emite um novo ingresso (realiza as buscas nos serviços de Eventos e Clientes)
  * `PATCH /api/ingressos/{id}/confirmar` - Atualiza o status do ingresso para `Confirmado`

#### 4. `Pagamentos-Service`
* **Responsabilidade:** Registro de transações financeiras e orquestração da aprovação de compras (simulada/mockada).
* **Base de Dados:** SQLite (`pagamentos.db`)
* **Entidade Principal:**
  ```csharp
  public class Pagamento
  {
      public int Id { get; set; }
      public int IngressoId { get; set; }
      public decimal Valor { get; set; }
      public string MetodoPagamento { get; set; } = "PIX"; // PIX, CartaoCredito, Boleto
      public string Status { get; set; } = "Aprovado";
      public DateTime DataPagamento { get; set; } = DateTime.UtcNow;
  }
  ```
* **Endpoints:**
  * `POST /api/pagamentos` - Registra um pagamento e dispara a confirmação no `Ingressos-Service`
  * `GET /api/pagamentos/{id}` - Consulta dados de um pagamento
  * `GET /api/pagamentos/ingresso/{ingressoId}` - Consulta pagamentos vinculados a um ingresso

---

## 3. Mapeamento das Integrações Obrigatórias

Conforme as diretrizes da avaliação, foram implementadas as seguintes integrações HTTP/REST entre os microsserviços:

### 3.1. Busca Simples 1: Validação do Evento
* **Origem:** `Ingressos-Service`
* **Destino:** `Eventos-Service`
* **Endpoint Consultado:** `GET /api/eventos/{eventoId}`
* **Finalidade:** Antes de emitir o ingresso, o `Ingressos-Service` consulta o `Eventos-Service` para confirmar se o evento existe e está com `Ativo == true`. Caso contrário, a criação é recusada com `400 Bad Request`.

### 3.2. Busca Simples 2: Validação Cadastral do Cliente
* **Origem:** `Ingressos-Service`
* **Destino:** `Clientes-Service`
* **Endpoint Consultado:** `GET /api/clientes/{clienteId}`
* **Finalidade:** O `Ingressos-Service` consulta o `Clientes-Service` para validar a existência cadastral do comprador antes de emitir a reserva do ingresso.

### 3.3. Alteração de Dados: Confirmação de Pagamento
* **Origem:** `Pagamentos-Service`
* **Destino:** `Ingressos-Service`
* **Endpoint Disparado:** `PATCH /api/ingressos/{id}/confirmar`
* **Finalidade:** Ao registrar e aprovar uma transação no `Pagamentos-Service`, este microsserviço envia uma requisição HTTP para o `Ingressos-Service`, atualizando o status do registro de `"Pendente"` para `"Confirmado"`.

---

## 4. Publicação e Links de Acesso

| Microsserviço | Diretório | URL Render | Swagger UI |
| :--- | :--- | :--- | :--- |
| **Eventos-Service** | `/src/EventosService` | `à ser subido` | `/swagger` |
| **Clientes-Service** | `/src/ClientesService` | `à ser subido` | `/swagger` |
| **Ingressos-Service** | `/src/IngressosService` | `à ser subido` | `/swagger` |
| **Pagamentos-Service** | `/src/PagamentosService` | `à ser subido` | `/swagger` |

---

## 5. Integrantes da Equipe
* **Aluno 1** - Kaio Souza Cadorin
* **Aluno 2** - Gustavo Olivo
* **Aluno 3** - Gabriel Tomazi
