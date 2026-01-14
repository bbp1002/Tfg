# API REST en .NET para la Gestión de Explotaciones Agrícolas y Planificación de Rotación de Cultivos

## Descripción del proyecto

Este proyecto consiste en una **API REST desarrollada en ASP.NET Core** para la gestión de explotaciones agrarias y el apoyo al cumplimiento de los requisitos de la **Política Agraria Común (PAC)**.

La aplicación permite importar datos oficiales del SIGPAC desde Excel, gestionar parcelas y recintos, mantener un histórico de cultivos por campaña y generar **propuestas de cultivo asistidas por Inteligencia Artificial**, teniendo en cuenta criterios agronómicos y normativos.

El proyecto nace a partir de una necesidad real detectada en el entorno familiar del autor, donde la gestión de la PAC se realiza de forma manual, con múltiples documentos en papel y búsquedas repetitivas en el visor SIGPAC, lo que supone una elevada carga administrativa y riesgo de errores.

---

## Objetivos principales

- Digitalizar la gestión de parcelas y recintos agrícolas.
- Automatizar la importación de datos PAC desde Excel.
- Mantener un histórico de cultivos por campaña.
- Generar propuestas de cultivo mediante IA.
- Facilitar el cumplimiento de criterios PAC (BCAM y ecorregímenes).
- Reducir el trabajo manual y los errores administrativos.

---

## Tecnologías utilizadas

**Backend**
- ASP.NET Core Web API  
- Entity Framework Core  
- PostgreSQL / pgAdmin 4  
- JWT Authentication  

**Procesamiento de datos**
- ClosedXML (Excel)

**Inteligencia Artificial**
- Google Gemini (modelo Flash 2.5)
- Prompt estructurado en JSON
- Una llamada por campaña

**Frontend**
- React

---

## Funcionalidades principales

- Importación PAC desde Excel.
- Gestión de parcelas y recintos.
- Histórico de cultivos por campaña.
- Generación de propuestas de cultivo con IA.
- Exportación de propuestas a Excel.
- Enlace directo al visor SIGPAC.
- Nombres personalizados de parcelas.
- Autenticación mediante JWT.

---

## Ejecución del proyecto

### Requisitos
- .NET SDK  
- PostgreSQL  
- Node.js  
- Clave de acceso a Google Gemini API  

---

## Documentación

- Memoria del TFG
- Anexo técnico con detalles de diseño y programación.

---

## Líneas futuras 

- Interfaz gráfica más avanzada.
- Comparación entre campañas agrícolas.
- Integración con datos meteorológicos. 
- Mejora del sistema de recomendaciones IA.
- Soporte para múltiples explotaciones por usuario.

---

Proyecto desarrollado como Trabajo de Fin de Grado con fines académicos.
