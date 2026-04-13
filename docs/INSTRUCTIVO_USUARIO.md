# Instructivo de Uso

## Alfa Sync Dashboard

Alfa Sync Dashboard es una aplicación para controlar la sincronización de datos entre la base central y los puntos de venta.

La app permite:

- cargar los locales desde la base central
- probar la conexión a cada punto de venta
- analizar diferencias entre central y local
- enviar precios y costos
- enviar todo el conjunto de datos definido para la sincronización
- ver el estado y el log del proceso en tiempo real

## Antes de usar

Antes de comenzar, verificar:

- que la conexión a la base central esté bien configurada
- que cada punto de venta tenga correctamente cargado `Server`, `Base`, `Usuario` y `Password`
- que la aplicación pueda conectarse tanto a la base central como a las bases locales

## Pantalla principal

En la grilla principal se muestran los locales disponibles y su información:

- `Sel`: permite marcar uno o más locales para trabajar
- `Código`: código del punto de venta
- `Sucursal`: número o identificador de sucursal
- `Descripción`: nombre visible del local
- `Server`: servidor SQL del local
- `Base`: base de datos del local
- `ScriptSet`: grupo lógico de sincronización asignado
- `Conexión`: resultado de la prueba de conexión
- `Última sync`: fecha y hora de la última sincronización exitosa
- `Estado`: estado actual del local dentro de la app

En la parte inferior:

- la barra de estado muestra el avance general
- el área de log muestra mensajes del proceso en tiempo real

## Qué hace cada botón

### `Actualizar locales`

Vuelve a leer los locales desde la vista central `V_TA_TPV`.

Usar este botón cuando:

- se agregaron nuevos locales
- se modificó algún dato de conexión
- se quiere refrescar la grilla

### `Configuración`

Abre la pantalla de configuración general de la aplicación.

Desde ahí se revisa:

- la conexión a la base central
- la ruta de scripts

Si se cambia la configuración, conviene cerrar y volver a abrir la app para asegurar que todo quede recargado correctamente.

### `Probar conexiones`

Intenta conectarse a cada local usando los datos de `Server`, `Base`, `Usuario` y `Password` del punto de venta.

Resultado esperado:

- `OK` si la conexión al local funciona
- `ERROR` si la conexión falla

Este paso no sincroniza nada. Solo valida conectividad.

### `Analizar seleccionados`

Compara los locales seleccionados contra la base central.

El análisis revisa diferencias en:

- familias
- artículos
- cabeceras de listas de precios
- precios

Sirve para saber si el local está alineado con central antes de enviar datos.

Este botón no modifica información. Solo informa diferencias.

### `Enviar precios y costos`

Sincroniza únicamente:

- artículos
- cabeceras de precios
- precios

Es la opción recomendada cuando se quiere actualizar precios y costos sin enviar familias.

### `Enviar todo`

Sincroniza:

- familias
- artículos
- cabeceras de precios
- precios

Usar esta opción cuando se necesita una actualización completa del local.

### `Cancelar`

Solicita detener el proceso actual.

Si la sincronización está en curso, la app intentará cancelar en el punto más seguro posible.

## Flujo recomendado de uso

Para minimizar errores, se recomienda este orden:

1. Abrir la app.
2. Hacer clic en `Actualizar locales`.
3. Hacer clic en `Probar conexiones`.
4. Marcar un solo local en `Sel`.
5. Hacer clic en `Analizar seleccionados`.
6. Si el análisis es correcto, hacer clic en `Enviar precios y costos`.
7. Revisar el log y el estado final.
8. Recién después repetir con más locales o usar `Enviar todo` si corresponde.

## Recomendación para primer uso

La primera vez conviene:

- trabajar con un solo local
- probar primero `Enviar precios y costos`
- validar el resultado en el local
- luego ampliar al resto

## Cómo interpretar el log

En la parte inferior de la ventana se muestran mensajes con hora.

Ejemplos de uso del log:

- confirmar que un local fue tomado por el proceso
- ver en qué etapa está trabajando
- detectar errores de conexión o de actualización
- confirmar cuántas filas fueron procesadas

## Estados habituales

Algunos estados que puede mostrar la app:

- `Listo`: el local está cargado y sin proceso en curso
- `Pendiente`: todavía no se probó la conexión
- `Sincronizando...`: se está ejecutando una sincronización
- `OK`: el proceso terminó correctamente
- `ERROR`: ocurrió un problema durante la conexión, análisis o sincronización
- `Cancelado`: el usuario canceló el proceso

## Buenas prácticas

- no ejecutar sincronizaciones masivas sin antes probar con un local
- revisar siempre `Conexión` antes de enviar datos
- usar `Analizar seleccionados` antes de sincronizar si hay dudas
- no cerrar la app mientras una sincronización esté en curso
- revisar el log ante cualquier error

## Qué no hace cada acción

- `Probar conexiones` no actualiza datos
- `Analizar seleccionados` no modifica datos
- `Cancelar` no borra información ya sincronizada

## Soporte ante errores

Si aparece un error:

1. revisar el mensaje exacto en el log
2. verificar los datos de conexión del local
3. probar acceso manual a la base del local
4. volver a ejecutar con un solo local seleccionado

Si el error persiste, registrar:

- nombre del local
- hora del error
- botón utilizado
- mensaje completo mostrado en el log

