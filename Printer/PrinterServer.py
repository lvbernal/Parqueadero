"""Just prints."""
# -*- coding: utf-8 -*-


import cherrypy
import datetime
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
        tokenize
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
    """

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

        p = printer.Usb(0x28e9, 0x0289, 0, 0x81, 0x03)
        p.text("\n")
        p.text("Parqueadero Alquim\n")
        p.text("Maribel Uribe. NIT 31.945.821-9\n")
        p.text("Carrera 5 con Calle 16\n\n")

        p.text(v_str + " " + plate + "\n")

        if helmets > 0:
            p.text("Cascos: " + str(helmets) + "\n")

        p.text("\nIngreso: " + ci_str + "\n")

        if complete:
            checkout = query.get("checkout", "")
            co_str = self._format_date(checkout)
            p.text("Salida:  " + co_str + "\n\n")
            p.text("$" + str(fee))
        else:
            p.text("\n")
            p.qr(plate, size=8)

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
