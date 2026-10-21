// Datos iniciales para la prueba técnica: 3 clientes con facturas en distintos estados.
// Se ejecuta automáticamente al crear el contenedor por primera vez.
// Los clientes se identifican por su NIT o cédula y las facturas por su número.

db = db.getSiblingDB("monolegal");

db.createCollection("clientes", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["_id", "nombre", "email"],
      properties: {
        _id: { bsonType: "string", description: "NIT o cédula" },
        nombre: { bsonType: "string" },
        email: { bsonType: "string", pattern: "^.+@.+\\..+$" },
      },
    },
  },
});

db.createCollection("facturas", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["_id", "clienteId", "monto", "estado", "fechaEmision", "fechaVencimiento"],
      properties: {
        _id: { bsonType: "string", description: "Número de factura" },
        clienteId: { bsonType: "string", description: "NIT o cédula del cliente" },
        monto: { bsonType: "decimal" },
        estado: { enum: ["primerrecordatorio", "segundorecordatorio", "desactivado", "pagada"] },
        fechaEmision: { bsonType: "date" },
        fechaVencimiento: { bsonType: "date" },
      },
    },
  },
});

db.facturas.createIndex({ clienteId: 1 });
db.facturas.createIndex({ estado: 1 });

db.clientes.insertMany([
  { _id: "900123456", nombre: "Inversiones Andinas S.A.S.", email: "cartera@andinas.test" },
  { _id: "1020304050", nombre: "Laura Gómez Restrepo", email: "laura.gomez@correo.test" },
  { _id: "800987654", nombre: "Tecnologías del Caribe Ltda.", email: "pagos@tecaribe.test" },
]);

const factura = (numero, clienteId, monto, estado, emision, vencimiento) => ({
  _id: numero,
  clienteId,
  monto: NumberDecimal(monto),
  estado,
  fechaEmision: ISODate(emision),
  fechaVencimiento: ISODate(vencimiento),
});

db.facturas.insertMany([
  factura("FAC-1001", "900123456", "1250000.00", "primerrecordatorio", "2026-08-01", "2026-08-31"),
  factura("FAC-1002", "900123456", "830000.00", "segundorecordatorio", "2026-07-01", "2026-07-31"),
  factura("FAC-1003", "900123456", "450000.00", "pagada", "2026-06-01", "2026-06-30"),

  factura("FAC-2001", "1020304050", "180000.00", "primerrecordatorio", "2026-09-01", "2026-09-30"),
  factura("FAC-2002", "1020304050", "95000.00", "desactivado", "2026-05-01", "2026-05-31"),

  factura("FAC-3001", "800987654", "2400000.00", "segundorecordatorio", "2026-07-15", "2026-08-14"),
  factura("FAC-3002", "800987654", "1100000.00", "primerrecordatorio", "2026-09-10", "2026-10-10"),
  factura("FAC-3003", "800987654", "760000.00", "pagada", "2026-04-01", "2026-04-30"),
]);

print(`Seed completado: ${db.clientes.countDocuments()} clientes, ${db.facturas.countDocuments()} facturas`);
