# Test_Monolegal

Prueba técnica de vinculación a Monolegal: un proceso de recordatorios de pago para facturas guardadas en MongoDB, con API en .NET 10 y un dashboard en Angular.

> La arquitectura, las decisiones técnicas y los puntos de evaluación se describen en el documento de entrega que acompaña este repositorio. Este README explica cómo funciona y cómo ejecutarlo.

## Cómo funciona

El proceso de recordatorios busca las facturas en estado `primerrecordatorio` o `segundorecordatorio` y, por cada una:

1. Busca al cliente dueño de la factura (por su NIT o cédula).
2. Le envía un correo según el estado de la factura.
3. **Solo si el correo se envió**, actualiza el estado en MongoDB.

| Estado actual | Correo enviado | Nuevo estado |
| --- | --- | --- |
| `primerrecordatorio` | La factura pasó a segundo recordatorio | `segundorecordatorio` |
| `segundorecordatorio` | El servicio va a ser desactivado | `desactivado` |

- Las facturas en `pagada` o `desactivado` no se modifican.
- Si el correo de una factura falla, esa factura queda igual y el proceso sigue con las demás.
- Al terminar, el proceso devuelve el resultado de cada factura (estado anterior, estado nuevo o el error).

El proceso se lanza desde el botón **Procesar recordatorios** del dashboard o con `POST /api/recordatorios/procesar`. Cada ejecución avanza un paso: si se ejecuta dos veces, las facturas que pasaron a segundo recordatorio quedan desactivadas en la segunda.

### Datos de prueba

La base se carga automáticamente la primera vez que arranca el contenedor de MongoDB ([docker/mongo-init/01-seed.js](docker/mongo-init/01-seed.js)):

| Cliente | NIT / cédula | Facturas |
| --- | --- | --- |
| Inversiones Andinas S.A.S. | 900123456 | FAC-1001 (primer recordatorio), FAC-1002 (segundo recordatorio), FAC-1003 (pagada) |
| Laura Gómez Restrepo | 1020304050 | FAC-2001 (primer recordatorio), FAC-2002 (desactivado) |
| Tecnologías del Caribe Ltda. | 800987654 | FAC-3001 (segundo recordatorio), FAC-3002 (primer recordatorio), FAC-3003 (pagada) |

En la primera ejecución se procesan 5 facturas y se envían 5 correos.

## Ejecución rápida (Docker)

Requisito: Docker Desktop abierto. En una terminal ubicada en la carpeta del proyecto:

```bash
docker compose up -d --build
```

| Servicio | URL |
| --- | --- |
| Dashboard (Angular) | http://localhost:8080 |
| API + Swagger | http://localhost:5080/swagger |
| Correos enviados (Mailpit) | http://localhost:8025 |
| MongoDB | `mongodb://admin:admin123@localhost:27017` |

Para probarlo: abre el dashboard, pulsa **Procesar recordatorios**, confirma y revisa los correos en Mailpit.

Para volver a los datos iniciales:

```bash
docker compose down -v
```

```bash
docker compose up -d
```

## Ejecución para desarrollo

Requisitos: .NET SDK 10 (incluido en Visual Studio 2026), Node.js 22 y Docker Desktop.

1. Levantar solo MongoDB y Mailpit:
   ```bash
   docker compose up -d mongo mailpit
   ```
2. API: abrir `Monolegal.sln` en Visual Studio, elegir `Monolegal.Api` como proyecto de inicio y ejecutar con el perfil `http` (abre Swagger en http://localhost:5080/swagger). Desde la terminal:
   ```bash
   dotnet run --project src/Monolegal.Api --launch-profile http
   ```
3. Frontend (http://localhost:4200, con proxy de `/api` hacia `localhost:5080`):
   ```bash
   cd frontend
   npm ci
   npm start
   ```

> Usa `npm ci` (no `npm install`): con npm 10 `npm install` falla por un error interno de npm al resolver dependencias; `npm ci` instala desde el `package-lock.json` sin ese problema.

## API

| Método | Ruta | Descripción |
| --- | --- | --- |
| `GET` | `/api/facturas` | Resumen de todas las facturas con los datos de su cliente |
| `POST` | `/api/recordatorios/procesar` | Envía los correos y actualiza los estados; devuelve el resultado por factura |

Se pueden probar desde Swagger (http://localhost:5080/swagger) o con Postman:

1. En Postman: **Import** → seleccionar [postman/Monolegal.postman_collection.json](postman/Monolegal.postman_collection.json).
2. Ejecutar las peticiones de la colección **Monolegal API**. La variable `baseUrl` apunta a `http://localhost:5080`.
3. Cada petición trae tests que se ven en la pestaña **Test Results**: código 200, totales que cuadran y transiciones de estado correctas.

La colección también se puede ejecutar desde la terminal con Newman:

```bash
npx newman run postman/Monolegal.postman_collection.json
```

## Pruebas unitarias

```bash
dotnet test
```

```bash
cd frontend
npx ng test --watch=false
```

- **Backend:** 19 tests con xUnit y Moq (proceso de recordatorios, plantillas de correo, resumen de facturas y controllers).
- **Frontend:** 14 tests con Vitest (filtros y totales, servicio HTTP y dashboard).

## Estructura del proyecto

```
src/Monolegal.Api/
  Controllers/    Reciben las peticiones HTTP y llaman al servicio correspondiente
  Services/       El proceso de recordatorios, el resumen de facturas y el envío de correos
  Repositories/   Acceso a MongoDB (consultar facturas y clientes, actualizar estados)
  Models/         Factura, Cliente, estados y objetos de respuesta
  Settings/       Clases que se llenan con appsettings.json (Mongo y SMTP)
  Program.cs      Configuración e inyección de dependencias
tests/Monolegal.UnitTests/   Tests del backend
frontend/                    Dashboard en Angular
postman/                     Colección de Postman
docker/mongo-init/           Script con los datos iniciales
docker-compose.yml           MongoDB, Mailpit, API y frontend
```

## Configuración del correo

Por defecto los correos van a Mailpit, que los captura sin enviarlos de verdad. Para usar Gmail, configurar la sección `Smtp` (por variables de entorno o User Secrets, nunca en el repositorio):

```json
"Smtp": {
  "Host": "smtp.gmail.com",
  "Puerto": 587,
  "UsarTls": true,
  "Usuario": "tu-cuenta@gmail.com",
  "Password": "<contraseña de aplicación>",
  "RemitenteEmail": "tu-cuenta@gmail.com",
  "RemitenteNombre": "Cartera"
}
```

Las credenciales de `appsettings.json` y `docker-compose.yml` son solo para el entorno local.
