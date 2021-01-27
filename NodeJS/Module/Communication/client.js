var mqtt = require('mqtt')
var client = mqtt.connect('mqtt://test.mosquitto.org')

client.on('connect', function () {
    client.subscribe('presence', function (err) {
        if (!err) {
            client.publish('presence', 'Hello mqtt')
        }
    })
})

client.on('close', function () {
    console.log('disconnected')
})

client.on('message', function (topic, message) {
    // message is Buffer
    console.log(message.toString())
    client.end()
})
/*
import paho.mqtt.client as mqtt
import sys, traceback
import json
try:
	import thread
except ImportError:
	import _thread as thread

class Client:
	def __init__(self, broker_host, broker_port, id):
		self.in_topic = id + '/in'
		self.out_topic = id + '/out'
		self.dbg_topic = id + '/debug'
		self.msg_handler = msg_handler
		self.mqtt_client = mqtt.Client(client_id=id, clean_session=True, userdata=None, protocol=mqtt.MQTTv311, transport='tcp')
		self.mqtt_client.on_message = self.on_message
		self.mqtt_client.on_connect = self.on_connect
		self.mqtt_client.on_disconnect = self.on_disconnect
		self.mqtt_client.connect(broker_host, broker_port)
		self.mqtt_client.loop_forever()

	def on_connect(self, client, userdata, flags, rc):
		self.mqtt_client.subscribe(self.in_topic)
		print('connected')
		try:
			thread.start_new_thread(initialize, (self,))
		except:
			pass

	def on_disconnect(self, client, userdata, rc):
		self.mqtt_client.disconnect()

	def on_message(self, client, userdata, msg):
		try:
			self.msg_handler(json.loads(msg.payload), self)
		except Exception as e:
			print('CrosserPythonError: %s' % e)
			sys.exit()

	def debug(self, data):
		self.mqtt_client.publish(self.dbg_topic, data)

	def send(self, data):
		self.publish(data)

	def next(self, data):
		self.publish(data)

	def publish(self, data):
		self.mqtt_client.publish(self.out_topic, json.dumps(data))

def main():
	global module
	module = Client('127.0.0.1', 1883, '{this.InstanceId}')

if __name__== '__main__':
	main()
";
*/