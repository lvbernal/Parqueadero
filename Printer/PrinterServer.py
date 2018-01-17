"""Just prints."""
# -*- coding: utf-8 -*-


import datetime
from Queue import Queue
from threading import Thread
import cherrypy
import dateutil.parser
from escpos import printer


class PrinterServer(object):
    """Printer server.

    Input
        {
            'plate': 'ABC123',
            'fee': 0.0,
            'complete': False,
            'helmets': 2,
            'helmets_fee': 1000.0,
            'checkin': '2017-12-10T11:47:47.860149-05:00',
            'additional_hours': 0,
            'total_additional_fee': 0.0,
            'additional_fee': 2500.0,
            'vehicle': 'pickup',
            'base_fee': 2500.0,
            'checkout': '0001-01-01T00:00:00'
        }

    """
    def __init__(self):
        contract = (
            u"Contrato de aparcamiento de vehiculos automotores\n"
            u"Para los efectos del contrato de deposito de vehiculo que aqui se celebra, el depositante declara: "
            u"1. Que tiene asegurado el vehiculo contra todos los riesgos hasta por su valor comercial, comprometiendose a dirigir toda reclamacion a la respectiva compania de seguros. "
            u"2. Que conoce y aprueba las condiciones de seguridad que existen en el parqueadero. "
            u"3. Que en caso de cualquier siniestro del vehiculo, el depositario solo responde hasta por cincuenta mil pesos ($50.000) moneda corriente, como indemnizacion de perjuicio, que sera pagadera previa sentencia en firme que establezca la responsabilidad del depositario. "
            u"4. Que el depositario entregara el vehiculo a la persona que exhiba esta boleta de parqueo, sin mas indemnizacion ni responsabilidad para el depositario. "
            u"5. Que el depositario tenga derecho de retencion sobre el vehiculo para compensarse por las expensas en la conservacion de la cosa y los perjuicios que culpa del depositante haya ocasionado el deposito. "
            u"6. Que el depositario se ajusta a los reglamentos oficiales aplicables a la operacion del parqueadero.\n"
            u"Las clausulas anteriores fueron discutidas libremente y las acepta el depositante por el solo hecho de parquear su vehiculo y no requiere de su firma para su validez.\n"
            u"El parqueadero no se hace responsable por cascos y chalecos que se dejen en la moto."
        )
        self.contract_message = contract.encode("GB18030")
        self.queue = Queue()

    @cherrypy.expose
    @cherrypy.tools.json_in()
    def printreceipt(self):
        """Print endpoint."""
        query = cherrypy.request.json
        self.queue.put_nowait(query)
        return "Ok"

    def _print(self, item):
        vehicle = item.get("vehicle", "")
        v_str = self._get_vehicle_str(vehicle)
        plate = item.get("plate", "")
        helmets = item.get("helmets", 0)
        checkin = item.get("checkin", "")
        ci_str = self._format_date(checkin)
        complete = item.get("complete", False)
        fee = int(item.get("fee", 0))

        base_fee = int(item.get("base_fee", 0))
        additional_fee = int(item.get("additional_fee", 0))
        additional_hours = int(item.get("additional_hours", 0))
        total_additional_fee = int(item.get("total_additional_fee", 0))
        helmets_fee = int(item.get("helmets_fee", 0))

        prt = printer.Usb(0x4b43, 0x3538, 0, 0x82, 0x02)
        prt.set(align='center')

        prt.text("\n")
        prt.text("Parqueadero Alquim\n")
        prt.text("NIT 31.945.821-9\n")
        prt.text("Carrera 5 con Calle 16\n")
        prt.text("Cali, Colombia\n\n")

        prt.set(align='center', width=2, height=2)
        prt.text(v_str + " " + plate + "\n")
        prt.set(align='center', width=1, height=1)

        if helmets > 0:
            prt.text("Cascos: " + str(helmets) + "\n")

        prt.text("\nIngreso: " + ci_str + "\n")

        if complete:
            checkout = item.get("checkout", "")
            co_str = self._format_date(checkout)
            prt.text("Salida:  " + co_str + "\n\n")

            prt.set(align='left')
            prt.text("Base: \t\t$" + str(base_fee) + "\n")
            prt.text("Hora adicional (" + str(additional_hours) + "): \t\t$" + str(total_additional_fee) + "\n")

            if helmets > 0:
                prt.text("Accesorios: \t\t$" + str(helmets_fee) + "\n")

            prt.text("\n")
            prt.set(align='center', width=2, height=2)
            prt.text("$" + str(fee))
            prt.set(align='center', width=1, height=1)
        else:
            prt.text("\n")
            prt.set(align='left')
            prt.text("Base: \t\t$" + str(base_fee) + "\n")
            prt.text("Hora adicional: \t\t$" + str(additional_fee) + "\n")

            if helmets > 0:
                prt.text("Accesorios (c/u): \t\t$" + str(helmets_fee) + "\n")

            prt.text("\n")
            prt.qr(plate, size=8)
            prt.text("\n\n")
            prt._raw(self.contract_message)

        prt.cut()

    def _format_date(self, date):
        dt = dateutil.parser.parse(date)
        return datetime.datetime.strftime(dt, "%d-%m-%Y %I:%M:%S %p")

    def _get_vehicle_str(self, vehicle):
        vehicle_map = {
            "car": "CARRO",
            "pickup": "CAMIONETA",
            "truck": "CAMION",
            "motorbike": "MOTO",
            "bike": "BICICLETA"
        }

        return vehicle_map[vehicle]

    def start_worker(self):
        """Run the worker that reads the queue and prints."""
        while True:
            if not self.queue.empty():
                item = self.queue.get()
                self._print(item)


if __name__ == "__main__":
    CHERRYPY_CONFIG = {
        "server.socket_host": "0.0.0.0",
        "server.socket_port": 80,
        "log.access_file": 'log_cherry.log',
        "log.error_file": 'log_error_cherry.log'
    }
    cherrypy.config.update(CHERRYPY_CONFIG)

    SERVER = PrinterServer()
    WORKER = Thread(target=SERVER.start_worker)

    WORKER.start()
    cherrypy.quickstart(SERVER)
