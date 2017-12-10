"""Just prints."""
# -*- coding: utf-8 -*-


import datetime
import cherrypy
import dateutil.parser
from escpos import printer


class Printer(object):
    """Printer server.
    Deps (apt):
        python-dev
        python-setuptools
        python-pip
        libjpeg-dev (Raspbian)
    Deps (pip):
        cherrypy
        python-escpos
        python-dateutil

    Config:
        lsusb
            Bus 001 Device 007: ID 28e9:0289
        lsusb -vvv -d 28e9:0289 | grep bEndpointAddress | grep IN
            bEndpointAddress  0x81 EP 1 IN
        lsusb -vvv -d 28e9:0289 | grep bEndpointAddress | grep OUT
            bEndpointAddress  0x03 EP 3 OUT
        Final config
            p = printer.Usb(0x28e9, 0x0289, 0, 0x81, 0x03)

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
            u"Para los efectos del contrato de depósito de vehículo que aquí se celebra, el depositante declara: "
            u"1. Que tiene asegurado el vehículo contra todos los riesgos hasta por su valor comercial, comprometiéndose a dirigir toda reclamación a la respectiva compañía de seguros. "
            u"2. Que conoce y aprueba las condiciones de seguridad que existen en el parqueadero. "
            u"3. Que en caso de cualquier siniestro del vehículo, el depositario sólo responde hasta por cincuenta mil pesos ($50.000) moneda corriente, como indemnización de perjuicio, que será pagadera previa sentencia en firme que establezca la responsabilidad del depositario. "
            u"4. Que el depositario entregará el vehículo a la persona que exhiba esta boleta de parqueo, sin más indemnización ni responsabilidad para el depositario. "
            u"5. Que el depositario tenga derecho de retención sobre el vehículo para compensarse por las expensas en la conservación de la cosa y los perjuicios que culpa del depositante haya ocasionado el depósito. "
            u"6. Que el depositario se ajusta a los reglamentos oficiales aplicables a la operación del parqueadero.\n"
            u"Las cláusulas anteriores fueron discutidas libremente y las acepta el depositante por el solo hecho de parquear su vehículo y no requiere de su firma para su validez.\n"
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

        p = printer.Usb(0x28e9, 0x0289, 0, 0x81, 0x03)
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
                p.text("Accesorios: \t\t$" + str(helmets_fee) + "\n")

            p.text("\n")
            p.qr(plate, size=8)
            p.text("\n\n")
            p._raw(self.contract_message)

        p.cut()
        return "Ok"

    def _format_date(self, date):
        dt = dateutil.parser.parse(date)
        return datetime.datetime.strftime(dt, "%d-%m-%Y %H:%M:%S")

    def _get_vehicle_str(self, vehicle):
        vehicle_map = {
            "car": "CARRO",
            "pickup": "CAMIONETA",
            "truck": "CAMION",
            "motorbike": "MOTO",
            "bike": "BICICLETA"
        }

        return vehicle_map[vehicle]


if __name__ == "__main__":
    CHERRYPY_CONFIG = {
        "server.socket_host": "0.0.0.0",
        "server.socket_port": 8000
    }

    cherrypy.config.update(CHERRYPY_CONFIG)
    cherrypy.quickstart(Printer())
