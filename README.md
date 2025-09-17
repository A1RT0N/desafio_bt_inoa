# desafio_bt_inoa

## Objetivo
Avisar, via e-mail, caso a cotação de um ativo da B3 caia mais do que certo nível, ou suba acima de outro.

## API escolhida
**Twelve Data (REST)**  
- Suporte à B3 gratuito  
- Endpoint simples para obter preço atual  
- Mais informações em: https://twelvedata.com/docs#overview

## Arquitetura
- **Core**: regras de negócio.  
- **Infra**: provedores externos.  
- **App**: console.  

## E-mails
- **Assunto**:  
  - `[ALERTA] {TICKER} acima de {teto}`  
  - `[ALERTA] {TICKER} abaixo de {piso}`  
- **Corpo**: preço atual, horário, variação do dia, links úteis.  

## Features que considero importantes para o sistema
- Configuração externa (SMTP, e-mail destino, chave da API, intervalos de polling).
- Suporte a cooldown para evitar envio de e-mails em excesso.
- Aplicação escalável em aplicações futuras.


## Como rodar

### Dependências
- **.NET 8 SDK**: instale em https://dotnet.microsoft.com/download/dotnet/8.0  
  Verifique: `dotnet --version`
- **Twelve Data API key**: crie conta em https://twelvedata.com e copie sua chave.
- **Servidor SMTP** para envio de e-mails
  - Ex.: **Gmail** com **senha de app** (2FA obrigatório)

### Configuração
Crie e edite o arquivo `src/StockAlert.Infra/appsettings.json`:

```json
{
  "Email": {
    "FromName": "Stock Alert Bot",
    "FromAddress": "SEU_EMAIL@gmail.com",
    "ToAddress": "EMAIL_DESTINO@gmail.com",
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": 587,
      "UseSsl": true,
      "Username": "SEU_EMAIL@gmail.com",
      "Password": "SENHA_DE_APP"
    }
  },
  "QuoteProvider": {
    "PollingSeconds": 5,
    "CooldownMinutes": 10,
    "TwelveDataApiKey": "SUA_CHAVE_TWELVEDATA"
  }
}
```

### Execução

Após realizar os passos acima, faça:

```bash
dotnet restore
dotnet build
dotnet run --project src/StockAlert.App <ATIVO> <PRECO_VENDA> <PRECO_COMPRA>
```
Em que ATIVO, PRECO_VENDA, PRECO_COMPRA, sejam o ativo a ser obsertado, o preço de venda e o preço de compra, respectivamente. 


Exemplo de comando:

```bash
dotnet run --project src/StockAlert.App PETR4 22.67 22.59
```

### Teste

```bash
dotnet test
```

- Caso não queira executar com seus próprios dados, deixo em 'ayrton_application.tar.gz' a minha aplicação. Basta descompactar e rodar os comendos de execução.

Agradeço à Inoa pelo desafio. Foi bem interessante realizá-lo e poder ver seu funcionamento.