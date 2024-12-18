import requests

headers = {
'Content-Type': 'application/json'
}
url = "http://localhost:8081/Contact/546"
response = requests.request("DELETE", url, headers=headers, verify=False)

print("Requisição enviada!")
