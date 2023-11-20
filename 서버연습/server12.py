import requests

url = "https://qnzkcmhrpaqtbzhs.tunnel-pt.elice.io/proxy/5000/server_return"
response = requests.get(url)

print(response.text)

"""
https://qnzkcmhrpaqtbzhs.tunnel-pt.elice.io/proxy/5000/
"""
