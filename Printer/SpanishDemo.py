"""Just prints."""
# -*- coding: utf-8 -*-


from escpos import constants
from escpos import printer


if __name__ == "__main__":
        p = printer.Usb(0x28e9, 0x0289, 0, 0x81, 0x03)
        p.set(align='center')
        msg = u"á, é, í, ó, ú, ñ"

        msg = (
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

        msg = msg.encode("GB18030")
        # msg = msg.decode("GB18030").encode("ascii")
        # msg = u' '.join(msg).encode('utf-8').strip()
        # print msg
        # p.set(align='left')
        # p._raw('\x1b\x52\x12')  # Pag 15
        # p._raw('\x1b\x74\x00')  # Pag 15
        # p._raw('\x40\x5C\x7D')
        # p._raw('\x4A\xA4\x80\x08')

        # p._raw('\x0a\xa0')
        p._raw(msg)
        p.cut()
