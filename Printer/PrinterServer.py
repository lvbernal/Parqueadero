"""Just prints."""
# -*- coding: utf-8 -*-


import datetime
import cherrypy
from cherrypy.process.plugins import Daemonizer
from cherrypy.process.plugins import PIDFile
import dateutil.parser
from escpos import printer


class Printer(object):
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

    @cherrypy.expose
    @cherrypy.tools.json_in()
    def printreceipt(self):
        """Print endpoint."""
        query = cherrypy.request.json

        vehicle = query.get("vehicle", "")
        v_str = self._get_vehicle_str(vehicle)
        plate = query.get("plate", "")
        helmets = query.get("helmets", 0)
        checkin = query.get("checkin", "")
        ci_str = self._format_date(checkin)
        complete = query.get("complete", False)
        fee = int(query.get("fee", 0))

        base_fee = int(query.get("base_fee", 0))
        additional_fee = int(query.get("additional_fee", 0))
        additional_hours = int(query.get("additional_hours", 0))
        total_additional_fee = int(query.get("total_additional_fee", 0))
        helmets_fee = int(query.get("helmets_fee", 0))

        p = printer.Usb(0x4b43, 0x3538, 0, 0x82, 0x02)
        p.set(align='center')

        p.text("\n")
        p.text("Parqueadero Alquim\n")
        p.text("NIT 31.945.821-9\n")
        p.text("Carrera 5 con Calle 16\n")
        p.text("Cali, Colombia\n\n")

        p.set(align='center', width=2, height=2)
        p.text(v_str + " " + plate + "\n")
        p.set(align='center', width=1, height=1)

        if helmets > 0:
            p.text("Cascos: " + str(helmets) + "\n")

        p.text("\nIngreso: " + ci_str + "\n")

        if complete:
            checkout = query.get("checkout", "")
            co_str = self._format_date(checkout)
            p.text("Salida:  " + co_str + "\n\n")

            p.set(align='left')
            p.text("Base: \t\t$" + str(base_fee) + "\n")
            p.text("Hora adicional (" + str(additional_hours) + "): \t\t$" + str(total_additional_fee) + "\n")

            if helmets > 0:
                p.text("Accesorios: \t\t$" + str(helmets_fee) + "\n")

            p.text("\n")
            p.set(align='center', width=2, height=2)
            p.text("$" + str(fee))
            p.set(align='center', width=1, height=1)
        else:
            p.text("\n")
            p.set(align='left')
            p.text("Base: \t\t$" + str(base_fee) + "\n")
            p.text("Hora adicional: \t\t$" + str(additional_fee) + "\n")

            if helmets > 0:
                p.text("Accesorios (c/u): \t\t$" + str(helmets_fee) + "\n")

            p.text("\n")
            p.qr(plate, size=8)
            p.text("\n\n")
            p._raw(self.contract_message)

        p.cut()
        return "Ok"

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

    @staticmethod
    def deamonize():
        """Run cherrypy instance in background."""
        d = Daemonizer(cherrypy.engine)
        d.subscribe()
        PIDFile(cherrypy.engine, 'daemon.pid').subscribe()


if __name__ == "__main__":
    CHERRYPY_CONFIG = {
        "server.socket_host": "0.0.0.0",
        "server.socket_port": 80,
        "log.access_file": 'log_cherry.log',
        "log.error_file": 'log_error_cherry.log'
    }

    cherrypy.config.update(CHERRYPY_CONFIG)
    Printer.deamonize()
    cherrypy.quickstart(Printer())
