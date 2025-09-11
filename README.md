# desafio_bt_inoa

## Objetivo
Avisar, via e-mail, caso a cotação de um ativo da B3 caia mais do que certo nível, ou suba acima de outro.

## API escolhida
**Twelve Data (REST)**  
- Suporte à B3 gratuito  
- Endpoint simples para obter preço atual  

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
- Logs com **Serilog**.  
- Nunca expor segredos no repositório 
- TLS obrigatório no envio de e-mails.  