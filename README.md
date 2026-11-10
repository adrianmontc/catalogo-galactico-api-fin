# Catálogo Galáctico API

API REST con controladores desarrollada en ASP.NET Core (.NET 10) y Entity Framework Core con MySQL. El proyecto evoluciona la versión original en Minimal API hacia una arquitectura en capas (`Controllers`, `Services`, `Data`, `Models`, `Dtos`), incorporando persistencia relacional, gestión de secretos con Secret Manager y protección de datos sensibles mediante hashing y cifrado simétrico.

## Qué hace el proyecto

El sistema administra un catálogo ambientado en una galaxia ficticia compuesto por cuatro entidades principales:

- **Personajes**: Registro de personajes con su especie, facción (`Rebelde`, `Imperio`, `Neutral`), afiliación, sensibilidad a la Fuerza y control de estado (`vivo`, `muerto`, `desconocido`).
- **Cards**: Estadísticas de combate asociadas en relación `1:1` a cada personaje (poder, habilidad especial, arma y nivel de peligrosidad).
- **Eventos**: Registro histórico de batallas (escala temporal BBY/ABY), participantes, bajas confirmadas, cálculo de MVP y simulación de enfrentamientos entre facciones.
- **Naves (Componente propio)**: Entidad vinculada mediante una relación `1:N` con `Personajes` (un piloto puede tener múltiples naves a su mando). Incluye protección de datos sensibles:
  - `CodigoIgnicionHash`: Contraseña de arranque almacenada únicamente como hash irreversible con `PasswordHasher<Nave>`.
  - `CoordenadasHangar`: Ubicación del hangar almacenada cifrada en MySQL mediante `AesGcm` y entregada descifrada al consultar la API.

Además del CRUD completo de las cuatro entidades, incluye tres endpoints analíticos que cruzan la información de `Personajes`, `Cards`, `Eventos` y `Naves`:
- `GET /api/eventos/{id}/despliegue-flota`: Agrupa por facción las naves desplegadas por los participantes de un evento y calcula el poder aéreo combinado (poder de la carta del piloto + poder de fuego de la nave).
- `GET /api/personajes/{id}/expediente-piloto`: Genera la hoja de servicio del piloto con su flota descifrada, cantidad de batallas disputadas e índice de efectividad total.
- `GET /api/eventos/{id}/refuerzos-disponibles?poderMinimo=120`: Filtra pilotos vivos en el año del evento que no participaron en él, que cuentan con carta y nave asignada, y cuyo poder combinado supera el mínimo requerido.

---

## Requisitos para ejecutarlo

- **.NET 10 SDK** instalado.
- **MySQL Server 8.0** (o superior) corriendo localmente en el puerto `3306`.
- Herramienta global de Entity Framework Core (`dotnet-ef`).

---

## 1. Preparar la base de datos en MySQL

El primer paso es tener MySQL corriendo. Con el usuario `root` creamos la base de datos y el usuario del proyecto:

```sql
CREATE DATABASE catalogo_galactico_db CHARACTER SET utf8mb4;

CREATE USER 'galactico_user'@'localhost' IDENTIFIED BY 'ElijanUnaClaveSegura';

GRANT ALL PRIVILEGES ON catalogo_galactico_db.* TO 'galactico_user'@'localhost';
FLUSH PRIVILEGES;
```

Podemos cambiar de usuario y acceder a la base de datos para verificar que inicialmente no existan tablas:

```bash
mysql -u galactico_user -p catalogo_galactico_db
```

```sql
SHOW TABLES;
```

---

## 2. Clonar el proyecto e instalar paquetes

Clonar el repositorio y restaurar las dependencias dentro de la carpeta del proyecto:

```bash
git clone https://github.com/TU_USUARIO/NOMBRE_DE_TU_REPO.git
cd CatalogoGalactico
dotnet restore
```

En caso de necesitar instalar o verificar manualmente los paquetes de Entity Framework Core para MySQL:

```bash
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet build
```

---

## 3. Configurar los secretos con Secret Manager

El proyecto no almacena credenciales ni claves en `appsettings.json`. Toda la configuración sensible se gestiona mediante `dotnet user-secrets`.

### Guardar la cadena de conexión

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=catalogo_galactico_db;User=galactico_user;Password=SU_CLAVE_FICTICIA"
```

### Generar la clave de cifrado AES y guardarla en Secret Manager

La clave para `CifradoService` (`AesGcm`) debe ser de 32 bytes (256 bits), aleatoria y codificada en Base64.

**Windows (PowerShell):**

```powershell
$bytes = New-Object byte[] 32
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

**macOS y Linux:**

```bash
openssl rand -base64 32
```

Guardar la clave generada en Secret Manager:

```bash
dotnet user-secrets set "Cifrado:Clave" "PEGA_AQUI_TU_CLAVE_BASE64_DE_32_BYTES"
```

Para comprobar que ambos secretos quedaron registrados correctamente:

```bash
dotnet user-secrets list
```

---

## 4. Aplicar las migraciones en MySQL

Si no tienes instalada la herramienta de Entity Framework Core, instálala con:

```bash
dotnet tool install --global dotnet-ef --version 9.0.0
dotnet ef --version
```

El repositorio incluye el historial del esquema dividido en dos migraciones:
1. `Inicial`: Crea las tablas `Personajes`, `Cards` y `Eventos`.
2. `AgregarNavesYProteccionDatos`: Evoluciona el esquema agregando la tabla `Naves`, su relación `1:N` con `Personajes` y las columnas para hash y cifrado.

Para crear y actualizar las tablas en MySQL ejecuta:

```bash
dotnet ef database update
```

En caso de fallo de conexión local, puedes especificar la cadena directamente:

```bash
dotnet ef database update --connection "Server=127.0.0.1;Port=3306;Database=catalogo_galactico_db;Uid=galactico_user;Password=SU_CLAVE_FICTICIA;SslMode=none"
```

---

## 5. Ejecutar el proyecto y verificar la protección de datos en MySQL

Para iniciar el servidor:

```bash
dotnet run
```

Con la aplicación en ejecución, puedes acceder a la interfaz de Swagger en `/swagger` para probar los controladores.

Para demostrar en MySQL que el código de ignición se guarda como hash y las coordenadas del hangar se guardan cifradas:

```sql
SHOW TABLES;
DESCRIBE Naves;
SHOW CREATE TABLE Naves;

SELECT Id, Nombre, Modelo, CodigoIgnicionHash, CoordenadasHangar FROM Naves;
```

En la consola de MySQL se observará el hash generado por `PasswordHasher` y el texto cifrado en Base64, mientras que al consultar `GET /api/naves` o `GET /api/personajes/{id}/expediente-piloto` en la API, el campo `coordenadasHangar` se entregará descifrado.