import requests
import json

# Mensagem
message = {
    "id":312,
    "nome": "João da Silva",
    "telefone": 955447788,
    "ddd": 11,
    "email": "joao.silva@example.com",
    "estado": "SP",
    "cidade": "São Paulo"
}

payload = json.dumps(message)
print(payload)
headers = {
'Content-Type': 'application/json'
}
url = "http://localhost:8081/Contact"
response = requests.request("PUT", url, headers=headers, data=payload,verify=False)

print("Requisição enviada!")
