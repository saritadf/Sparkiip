# 🚀 Setup en 3 Pasos - AI Testing

## ✅ Lo que YA está hecho automáticamente:

- ✅ Escena de prueba creada: `Assets/Scenes/AITest.unity`
- ✅ UI completa configurada (panel, estilos, controlador)
- ✅ PanelSettings creado
- ✅ Scripts de AI listos
- ✅ Todo el código funcionando

## 🎯 Lo que TÚ necesitas hacer (5 minutos):

### Paso 1: Crear GPTAuth.asset (2 minutos) ⚠️ IMPORTANTE

Esta es la ÚNICA cosa que necesito que hagas manualmente (porque necesitas pegar tu API key):

1. **Abre Unity Editor**

2. **Ve a la carpeta Resources:**
   - En Project window: `Assets → Resources`

3. **Crea el asset:**
   - Click derecho en la carpeta Resources
   - Selecciona: **Create → ScriptableObjects → ApiAuthenticationSettings**
   - Nómbralo: **`GPTAuth`** (exactamente así, sin espacios)

4. **Configura el asset:**
   - Click en el archivo `GPTAuth` que acabas de crear
   - En Inspector, verás estos campos:
   
   ```
   Model: Selecciona "GPT_3_5_TURBO"
   Completion Url: (déjalo como está)
   Private Api Key: [PEGA TU API KEY AQUÍ]
   Organization: (déjalo vacío)
   Project Id: (déjalo vacío)
   ```

5. **Obtén tu API key:**
   - Ve a: https://platform.openai.com/api-keys
   - Haz login o crea cuenta (tienen $5 gratis para nuevos usuarios)
   - Click "Create new secret key"
   - Copia la key
   - Pégala en el campo "Private Api Key" del asset

6. **Guarda:**
   - Ctrl+S o File → Save

✅ **¡Listo! Esta es la única parte manual.**

---

### Paso 2: Abrir la escena (30 segundos)

1. En Unity Project window:
   - Ve a: `Assets → Scenes`
   - Doble-click en: **`AITest.unity`**

2. Verás en Hierarchy:
   - Main Camera
   - AI Test UI (con el componente AITestUIController)

---

### Paso 3: Probar (30 segundos)

1. **Click en Play ▶️**

2. **Verás la interfaz con 3 secciones:**
   - Daily Challenge
   - Sentiment Analysis
   - Empathy Suggestions

3. **Prueba los botones:**
   - "Generate Challenge" → Espera 1-2 segundos → Ve el desafío AI
   - "Analyze Sentiment" → Ve el score emocional (0.0-1.0)
   - "Get Response Suggestions" → Ve 3 respuestas empáticas

4. **Revisa Console (Ctrl+Shift+C):**
   - Verás logs detallados de cada respuesta AI

---

## 🎉 ¡Eso es TODO!

### Si ves esto, funciona perfectamente:

```
En la UI:
✅ Challenge: "Share a photo of your favorite place..."
✅ Sentiment Score: 0.82 (Deeply Emotional 💖)
✅ Bonsai Growth: +0.082 🌱
✅ 3 suggestions aparecer

En Console:
=== Generating Daily Challenge ===
✅ Challenge Generated: ...
=== Analyzing Sentiment ===
✅ Sentiment: 0.82 (Deeply Emotional)
=== Getting Empathy Suggestions ===
✅ Generated 3 suggestions
```

---

## 🐛 Problemas Comunes:

### Error: "Please set up authentication"
→ No creaste `GPTAuth.asset` o está mal nombrado
→ Debe estar en `Assets/Resources/GPTAuth.asset` exactamente

### Error: "401 Unauthorized"
→ API key incorrecta
→ Verifica que copiaste completa la key
→ Verifica que tienes créditos en OpenAI

### No aparece la UI
→ Asegúrate de abrir la escena `AITest.unity`
→ Verifica que Play esté presionado

### Botones no hacen nada
→ Revisa Console para errores
→ Verifica que GPTAuth.asset existe y tiene API key

---

## 💰 Costo de prueba:

Cada prueba completa cuesta ~$0.006 (menos de 1 centavo)

- Generate Challenge: $0.002
- Analyze Sentiment: $0.001
- Get Suggestions: $0.003

Puedes hacer 1000+ pruebas con $5 de crédito gratis.

---

## 📊 Qué probar:

### Daily Challenge:
- Click varias veces para ver diferentes challenges
- Cada uno será único y personalizado

### Sentiment Analysis:
Prueba estos mensajes para ver diferencias:

**Alta emoción:**
- "I miss you so much today, thinking about our last call"
- "Feeling so grateful for you in my life"

**Baja emoción:**
- "Had lunch today"
- "It's raining outside"

**Media emoción:**
- "Work was busy but good"
- "Looking forward to talking tonight"

### Empathy Suggestions:
Prueba con mensajes del partner como:

- "Feeling overwhelmed with work"
- "Missing home so much today"
- "Had a great day, got a promotion!"

---

## ✨ Siguiente Paso:

Una vez que todo funcione:

1. **Lee:** `AI_INTEGRATION_SETUP.md` para entender la arquitectura
2. **Integra:** Estas funciones en tu Dashboard principal
3. **Conecta:** Sentiment con sistema Bonsai
4. **Antes de Beta:** Migrar a Cloud Functions (seguridad)

---

## 🎯 Resumen Visual:

```
TÚ HACES (Solo Paso 1):
└── Crear GPTAuth.asset con tu API key (2 min)

YO YA HICE:
├── Escena AITest.unity ✅
├── UI completa configurada ✅
├── PanelSettings ✅
├── Controlador conectado ✅
└── Todo el código funcionando ✅

RESULTADO:
└── Click Play → UI funcional → AI responde! 🎉
```

---

**¿Listo?** Solo crea el GPTAuth.asset y dale Play! 🚀

