# 💼 Colpix API

API desarrollada en **.NET 8** con autenticación **JWT (Bearer)**.  
Este documento explica cómo probar los endpoints disponibles.

---

## 🌐 Base URL (desarrollo)

```
https://localhost:44397
```

> Ajustar según tu configuración y `launchSettings.json`.

---

## 🔐 Autenticación

### 🧾 Registrar usuario

**POST** `/Auth/register`

#### 📨 Solicitud
```json
{
  "username": "nuevoUsuario",
  "email": "nuevo@correo.com",
  "password": "123456"
}
```

#### ✅ Respuesta exitosa (`201 Created`)
```json
{
  "id": 12,
  "username": "nuevoUsuario"
}
```

#### ⚠️ Respuesta de error (`409 Conflict`)
El usuario o correo ya existe.

---

### 🔑 Obtener token

**POST** `/Auth/login`

#### 📨 Solicitud
```json
{
  "username": "colpix",
  "password": "colpix"
}
```

#### ✅ Respuesta
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5c...",
  "expires": "2025-11-05T22:45:00Z"
}
```

#### 🔧 Uso en peticiones protegidas
Agregá el encabezado HTTP:

```
Authorization: Bearer {token}
```

#### 🧠 Códigos comunes
| Código | Descripción |
|---------|--------------|
| 401 | Token faltante o inválido |
| 403 | Token válido pero sin permisos |

---

## 📦 Modelos relevantes

### 👤 `Employee` (respuesta / listados)
```json
{
  "id": 1,
  "name": "Nombre",
  "email": "correo@ejemplo.com",
  "supervisor_id": 0
}
```

### 📋 `EmployeeDetailsDto` (detalle)
```json
{
  "id": 5,
  "name": "Nombre",
  "email": "correo@ejemplo.com",
  "supervisor_id": 1,
  "lastUpdate": "2025-11-05T10:45:00Z",
  "reportsCount": 4
}
```

---

## ⚙️ Endpoints

### 📄 `GET /Employee`
Obtiene todos los empleados.

**Respuesta:** `200 OK` — array de `Employee`.

#### 🧪 Ejemplo cURL
```bash
curl -X GET http://localhost:44397/Employee \
  -H "Authorization: Bearer {token}"
```

---

### 🔍 `GET /Employee/{id}`
Obtiene el detalle de un empleado (incluye `lastUpdate` y `reportsCount`).

**Respuestas:**
- ✅ `200 OK` — `EmployeeDetailsDto`
- ❌ `404 Not Found` — empleado no encontrado

#### 🧪 Ejemplo cURL
```bash
curl -X GET http://localhost:44397/Employee/5 \
  -H "Authorization: Bearer {token}"
```

---

### ➕ `POST /Employee`
Crea un nuevo empleado.

#### 📨 Body JSON
```json
{
  "name": "Carlos López",
  "email": "carlos@empresa.com",
  "supervisor_id": 1
}
```

**Respuesta:** `200 OK` — texto:  
```
Empleado creado correctamente.
```

#### 🧪 Ejemplo cURL
```bash
curl -X POST http://localhost:44397/Employee \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"name":"Carlos López","email":"carlos@empresa.com","supervisor_id":1}'
```

---

### ✏️ `PUT /Employee?idEmpleado={id}`
Actualiza un empleado existente.

**Query string:** `idEmpleado` (int)

#### 📨 Body JSON
```json
{
  "name": "Carlos López",
  "email": "carlos.lopez@empresa.com",
  "supervisor_id": 1
}
```

**Respuestas:**
- ✅ `200 OK` — `"Empleado {id} modificado correctamente."`
- ❌ `404 Not Found` — no existe el empleado

#### 🧪 Ejemplo cURL
```bash
curl -X PUT "http://localhost:44397/Employee?idEmpleado=3" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"name":"Carlos López","email":"carlos.lopez@empresa.com","supervisor_id":1}'
```

---

## 🧠 Notas y recomendaciones

- Usar siempre `Content-Type: application/json` para `POST` y `PUT`.
- En Postman/Insomnia:  
  Añadir header → `Authorization: Bearer {token}`.
- Si recibís `401 Unauthorized`, solicitá un nuevo token en `/Auth/login` y verificá el campo `expires`.
- El controlador devuelve:
  - **Texto plano** en `POST`/`PUT`.
  - **JSON** en `GET`.
- Asegurate de que la API **y la base de datos** estén corriendo antes de probar.
- Para detalles de validaciones, revisar las clases:
  - `Employee`
  - `EmployeeDetailsDto`
  - `IEmployeeService`

---

## ⚡ Ejecución local rápida

```bash
dotnet restore
dotnet run
```

Luego probá los endpoints con **Swagger**, **Postman** o **cURL**.

---

📘 **Swagger UI:**  
[https://localhost:44397/swagger]
