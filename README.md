# Alfa Sync Dashboard

Proyecto WinForms para controlar el envío de artículos, precios y cabeceras de listas a los locales, manteniendo la lógica actual basada en scripts SQL.

## Qué hace esta versión

- Carga los locales desde `dbo.V_TA_TPV`
- Permite probar conexiones a cada local usando `SERVER / DBNAME / USUARIO / PASSWORD`
- Ejecuta **Artículos + PreciosCab + Precios** o **Familias + Artículos + PreciosCab + Precios**
- Muestra progreso general y por local
- Muestra log en vivo
- Guarda historial en `dbo.LOG_SYNC` (la tabla se crea automáticamente en la base central)
- Incluye un análisis básico de diferencias:
  - artículos faltantes
  - diferencias de costo
  - cabeceras faltantes
  - precios faltantes
  - diferencias de precios

## Importante sobre la lógica de ejecución

Para **preservar la forma de trabajo actual**, la ejecución de los scripts se hace contra la **base central** usando la connection string central configurada en `appsettings.json`.

Es decir, esta versión mantiene la lógica actual de scripts como:

- `ACTUALIZA_ARTICULOS.SQL`
- `ACTUALIZA_V_MA_PRECIOS.SQL`
- `ACTUALIZA_V_MA_PRECIOSCAB.SQL`

que ya contienen los linked servers y el comportamiento actual.

Las conexiones directas a cada local se usan para:

- probar conectividad
- analizar diferencias

## Requisitos

- Windows
- Visual Studio 2022 o superior
- .NET 8 SDK
- Acceso a SQL Server central
- Acceso a SQL Server de los locales

## Estructura

- `AlfaSyncDashboard.sln`
- `AlfaSyncDashboard/` proyecto WinForms
- `Scripts/` scripts base y copia del CMD actual

## Configuración inicial

Editar `AlfaSyncDashboard/appsettings.json`

### 1. Connection string central

```json
"CentralConnectionString": "Server=WIN-TUNPH1OHJM9\\ALFANET;Database=DISTRIWALTERP;User Id=DISTRIWALTERP;Password=DISTRIWALTERP;TrustServerCertificate=True;Encrypt=False;"
```

### 2. Ruta de scripts

Por default:

```json
"DefaultScriptsPath": "C:\\TAREASALFA"
```

Podés cambiarla desde la app en `Configuración`.

### 3. Mapeo de scripts por local

Ejemplo incluido:

- locales cuya descripción contiene `MALVINAS` => `MALVINAS`
- resto => `DEFAULT`

Si necesitás más variantes, agregalas en:

```json
"LocalScriptMappings"
```

y definí los nombres de archivo en:

```json
"ScriptSets"
```

## Cómo arrancar

1. Abrir `AlfaSyncDashboard.sln`
2. Restaurar paquetes NuGet
3. Ejecutar
4. Ir a `Configuración` y revisar:
   - connection string central
   - ruta base de scripts
5. Copiar los scripts SQL a `C:\TAREASALFA` o cambiar la ruta
6. Cargar locales
7. Probar conexiones
8. Analizar o sincronizar

## Recomendación de primer uso

1. Probar conexiones
2. Analizar un solo local
3. Ejecutar `Enviar precios y costos` sobre un solo local
4. Revisar `LOG_SYNC`
5. Luego ampliar al resto

## Limitaciones actuales

- La ejecución mantiene el enfoque actual basado en scripts SQL existentes
- El progreso es por etapas, no por fila interna del cursor
- El análisis de diferencias puede ser pesado si las tablas son muy grandes
- Si cambiás configuración de connection string, conviene reiniciar la app

## Próxima mejora sugerida

Optimizar los scripts SQL fila por fila para convertirlos a procesos set-based, empezando por:

1. `V_MA_PRECIOS`
2. `V_MA_ARTICULOS`

## Tabla de log

La app crea automáticamente:

```sql
IF OBJECT_ID('dbo.LOG_SYNC', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.LOG_SYNC
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Fecha DATETIME NOT NULL DEFAULT GETDATE(),
        Local NVARCHAR(100) NOT NULL,
        Proceso NVARCHAR(100) NOT NULL,
        Mensaje NVARCHAR(MAX) NULL,
        Estado NVARCHAR(20) NOT NULL
    );
END
```

