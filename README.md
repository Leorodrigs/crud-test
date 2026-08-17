# Link para as documentações

### http://localhost:5099/swagger
### http://localhost:5099/scalar
### http://localhost:5099/rapidoc
### http://localhost:5099/stoplight
### http://localhost:5099/redoc
### http://localhost:5099/openapi-explorer
### http://localhost:5099/mintlify

## Execução local

Inicie a API no diretório do projeto:

```bash
dotnet run
```

Em outro terminal, inicie o preview oficial do Mintlify (Node.js 20.17 ou superior):

```bash
cd mintlify
npx mint dev --local-schema --port 3000
```

Depois, acesse `http://localhost:5099/mintlify`. A API redireciona essa rota para o processo local do Mintlify em `http://localhost:3000`.

Para validar a configuração com a CLI oficial, mantenha a API em execução e rode dentro da pasta `mintlify`:

```bash
npx mint validate --local-schema
```
