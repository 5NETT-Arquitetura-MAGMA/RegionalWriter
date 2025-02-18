import json
import requests
import csv

# Caminho do arquivo CSV
csv_file_path = 'dados.csv'

# Ler o arquivo CSV e enviar mensagens ao RabbitMQ
with open(csv_file_path, newline='', encoding='utf-8') as csvfile:
    reader = csv.DictReader(csvfile)
    
    for row in reader:
        # Preencher o JSON com dados da linha do CSV
        message = {
            "nome": row.get("Nome", ""),
            "telefone": int(row.get("Telefone", 0)),
            "ddd": int(row.get("DDD", 0)),
            "email": row.get("Email", ""),
            "estado": row.get("Estado", ""),
            "cidade": row.get("Cidade", "")
        }
        payload = json.dumps(message)
        print(payload)
        headers = {
        'Content-Type': 'application/json'
        }
        url = "http://51.8.207.65:8080/Contact"
        response = requests.request("POST", url, headers=headers, data=payload,verify=False)

        print(response.text)
print("Requisição enviada!")
