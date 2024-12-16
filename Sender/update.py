import pika
import json

# Configurações do RabbitMQ
rabbit_host = 'localhost'
rabbit_user = 'regional'
rabbit_password = 'R3gional!234'

# Conexão com o RabbitMQ
credentials = pika.PlainCredentials(rabbit_user, rabbit_password)
connection = pika.BlockingConnection(pika.ConnectionParameters(host=rabbit_host, credentials=credentials))
channel = connection.channel()

# Mensagem
message = {
    "id":5,
    "nome": "João da Silva",
    "telefone": 955447788,
    "ddd": 11,
    "email": "joao.silva@example.com",
    "estado": "SP",
    "cidade": "São Paulo"
}

# Declaração da fila como durável
channel.queue_declare(queue='regional_update', durable=True)

# Adicionar cabeçalhos compatíveis com o MassTransit
headers = {
    "Content-Type": "application/json",
}

# Publicar mensagem com cabeçalhos
channel.basic_publish(
    exchange='',
    routing_key='regional_update',
    body=json.dumps(message),
    properties=pika.BasicProperties(
        delivery_mode=2,  # Faz a mensagem persistir
        headers=headers
    )
)

print("Mensagem enviada!")
connection.close()
