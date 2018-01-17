# Parqueadero

Aplicación para el negocio familiar, que permite registrar el ingreso y la salida de vehículos de un parqueadero y calcular el precio final.

La aplicación es comercial, pero he aprendido mucho haciéndola y quiero compartir el código para que otros puedan explorarla. Está desarrollada en Xamarin (C#) y almacena los datos en Azure. También incluye un script en Python que imprime recibos en una impresora ESC/POS genérica.

## Iconos y colores

* check in by Chinnaking from the Noun Project
* checkout by Chinnaking from the Noun Project
* clipboard by jeff from the Noun Project
* Settings by jeff from the Noun Project
* Car by jeff from the Noun Project
* Truck by jeff from the Noun Project
* Truck by jeff from the Noun Project
* ATV by jeff from the Noun Project
* Bicycle by jeff from the Noun Project
* Piggy Bank by jeff from the Noun Project
* Helmet by Gabriel Ciccariello from the Noun Project
* subtraction by Marta Ambrosetti from the Noun Project
* addition by Marta Ambrosetti from the Noun Project
* Equal by Marta Ambrosetti from the Noun Project
* Parking by Arthur Shlain from the Noun Project
* Copy of Picture book by Robert Lasch from Adobe Color CC
    * https://color.adobe.com/Copy-of-Picture-book-color-theme-10127683/

## Pendientes

* Manejo de concurrencia en la impresión: El script de la impresora está muy mal estructurado, allí me falta mucho trabajo.
* Autenticación y autorización: No quiero usar el modelo usuario/contraseña sino un "código de autorización".
* Internacionalización y manejo de cadenas de texto: Todos los textos están directamente en el código, en español.

## Configuración de la impresora

Instalar dependencias:

```
[sudo] apt-get install python-dev python-setuptools python-pip libjpeg-dev
[sudo] pip install cherrypy python-escpos python-dateutil
```

Ejecutar los siguientes comandos:

```
$ lsusb
Bus 001 Device 007: ID 28e9:0289

$ lsusb -vvv -d 28e9:0289 | grep bEndpointAddress | grep IN
bEndpointAddress  0x81 EP 1 IN

$ lsusb -vvv -d 28e9:0289 | grep bEndpointAddress | grep OUT
bEndpointAddress  0x03 EP 3 OUT
```

Con los valores (de ejemplo) **0x28e9**, **0x0289**, **0x81** y **0x03**, se inicializa la impresora:

```
p = printer.Usb(0x28e9, 0x0289, 0, 0x81, 0x03)
```

Configurar el inicio automático ejecutando ```[sudo] crontab -e``` y agregando:

```
@reboot (sleep 10; python /PATH_TO_FILE/PrinterServer.py)
```
