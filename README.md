# TicketPass - Sistema de Gestão de Eventos e Ingressos
> **Disciplina:** Engenharia de Software - Arquitetura de Software  
> **Professor:** Eduardo Cizeski Meneghel  
> **Instituição:** Centro Universitário SATC (UNISATC)  
> **Tecnologias:** .NET 8 (C#), SQLite, ASP.NET Core Minimal APIs / Web API, REST, Docker, Render  

---

## 1. Documento de Requisitos

### 1.1. Propósito do Sistema
O **TicketPass** é uma solução de back-end baseada em arquitetura de microsserviços voltada para a gestão de eventos culturais e corporativos, emissão e validação de ingressos e controlo de acesso aos recintos. O sistema permite criar eventos, gerir a disponibilidade de lotes de bilhetes, cadastrar compradores e validar a entrada no momento do evento, garantindo consistência e isolamento entre os domínios.

### 1.2. Usuários do Sistema
* **Organizador do Evento:** Cadastra eventos, define locais, datas e capacidade máxima de público.
* **Cliente / Comprador:** Consulta eventos disponíveis, adquire ingressos nominais e acompanha o histórico de compras.
* **Operador de Portaria / Check-in:** Realiza a leitura e validação do ingresso no momento da entrada do participante no evento.

### 1.3. Requisitos Funcionais (RF)
* **RF01 - Cadastro de Eventos:** O sistema deve permitir o cadastro, listagem e consulta detalhada de eventos (nome, data, local e lotação).
* **RF02 - Cadastro de Clientes:** O sistema deve permitir cadastrar e consultar dados cadastrais de compradores (nome, e-mail e documento).
* **RF03 - Emissão de Ingressos:** O sistema deve permitir a emissão de ingressos associados a um evento e a um cliente, garantindo que o evento exista e esteja ativo.
* **RF04 - Validação de Cliente na Compra:** Ao emitir um ingresso, o sistema deve validar se os dados do cliente comprador existem e estão regulares.
* **RF05 - Controlo de Acesso (Check-in):** O sistema deve validar se o ingresso apresentado é autêntico, pertence ao evento correto e ainda não foi utilizado.
* **RF06 - Atualização de Status do Ingresso:** Ao validar com sucesso o check-in na portaria, o sistema deve atualizar de forma automática e imediata o status do ingresso para `Utilizado`, impedindo reentradas com o mesmo bilhete.

---

## 2. Descritivo Técnico da Arquitetura

O sistema é dividido em **4 microsserviços independentes**, cada um com a sua própria base de dados SQLite local, expondo contratos REST documentados via Swagger:

```text
[ CheckIn-Service ] ─────────► (Alteração: PATCH /api/ingressos/{id}/utilizar) ────► [ Ingressos-Service ]
                                                                                           │          │
   ┌────────────────────── (Busca 1: GET /api/eventos/{id}) ───────────────────────────────┘          │
   ▼                                                                                                   ▼
[ Eventos-Service ]                                                                           [ Clientes-Service ]
                                                            (Busca 2: GET /api/clientes/{id}) ─────────┘
```

### 2.1. Microsserviços e Responsabilidades

#### 1. `Eventos-Service`
* **Responsabilidade:** Gerir o ciclo de vida dos eventos (criação, alteração de capacidade, encerramento).
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
  * `GET /api/eventos/{id}` - Busca os dados de um evento por ID
  * `POST /api/eventos` - Cria um novo evento

#### 2. `Clientes-Service`
* **Responsabilidade:** Gestão cadastral dos participantes e compradores.
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
* **Responsabilidade:** Emissão, armazenamento e ciclo de vida dos ingressos emitidos.
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
      public string Status { get; set; } = "Valido"; // Valido, Utilizado, Cancelado
      public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
  }
  ```
* **Endpoints:**
  * `GET /api/ingressos/{id}` - Consulta ingresso por ID
  * `POST /api/ingressos` - Emite um novo ingresso (realiza as buscas nos serviços de Eventos e Clientes)
  * `PATCH /api/ingressos/{id}/utilizar` - Altera o status do ingresso para `Utilizado`

#### 4. `CheckIn-Service`
* **Responsabilidade:** Operação de portaria e validação de ingressos na entrada do evento.
* **Base de Dados:** SQLite (`checkin.db`)
* **Entidade Principal:**
  ```csharp
  public class RegistroCheckIn
  {
      public int Id { get; set; }
      public int IngressoId { get; set; }
      public DateTime DataHoraEntrada { get; set; } = DateTime.UtcNow;
      public string Portaria { get; set; } = "Principal";
  }
  ```
* **Endpoints:**
  * `POST /api/checkin/validar` - Recebe o ID do ingresso, valida e dispara a alteração no `Ingressos-Service`
  * `GET /api/checkin/historico` - Histórico de entradas realizadas

---

## 3. Mapeamento das Integrações Obrigatórias

Conforme estipulado no edital, foram implementadas as seguintes comunicações HTTP/REST entre os serviços:

### 3.1. Busca Simples 1: Validação da Existência do Evento
* **Origem:** `Ingressos-Service`
* **Destino:** `Eventos-Service`
* **Endpoint Consultado:** `GET /api/eventos/{eventoId}`
* **Finalidade:** Antes de emitir o ingresso, o serviço consulta o evento para confirmar se ele existe e se está ativo (`Ativo == true`). Se não for encontrado, a requisição retorna `400 Bad Request`.

### 3.2. Busca Simples 2: Validação Cadastral do Cliente
* **Origem:** `Ingressos-Service`
* **Destino:** `Clientes-Service`
* **Endpoint Consultado:** `GET /api/clientes/{clienteId}`
* **Finalidade:** O serviço de ingressos consulta os dados do comprador para validar que o `ClienteId` fornecido é legítimo antes de consolidar a emissão.

### 3.3. Alteração de Dados: Baixa de Ingresso no Check-in
* **Origem:** `CheckIn-Service`
* **Destino:** `Ingressos-Service`
* **Endpoint Disparado:** `PATCH /api/ingressos/{id}/utilizar`
* **Finalidade:** Ao registrar a entrada física do participante na portaria, o `CheckIn-Service` dispara uma chamada HTTP que altera o estado do registro correspondente no banco do `Ingressos-Service` de `"Valido"` para `"Utilizado"`.

---

## 4. Publicação e Links de Acesso

| Microsserviço | Repositório / Diretório | URL Render | Swagger UI |
| :--- | :--- | :--- | :--- |
| **Eventos-Service** | `/src/EventosService` | `https://ticketpass-eventos.onrender.com` | `/swagger` |
| **Clientes-Service** | `/src/ClientesService` | `https://ticketpass-clientes.onrender.com` | `/swagger` |
| **Ingressos-Service** | `/src/IngressosService` | `https://ticketpass-ingressos.onrender.com` | `/swagger` |
| **CheckIn-Service** | `/src/CheckInService` | `https://ticketpass-checkin.onrender.com` | `/swagger` |

---

## 5. Integrantes da Equipe
* **Aluno 1** - Kaio
* **Aluno 2** - Gustavo Olivo
* **Aluno 3** - Gabriel Tomazi