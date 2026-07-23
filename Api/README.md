# 🤝 Relationship AI API

API leve e focada em conversas sobre relacionamentos, construída com C# e ASP.NET Core.

## 📋 Recursos

- ✅ API REST simples e eficiente
- ✅ IA especializada em relacionamentos (amorosos, familiares, amizades)
- ✅ Suporte a histórico de conversação
- ✅ CORS habilitado para apps mobile (Expo)
- ✅ Documentação Swagger integrada
- ✅ Modelo leve e rápido (Claude Haiku)
- ✅ Respostas em português

## 🚀 Como Rodar

### Pré-requisitos

- .NET 8.0 SDK ou superior
- API Key da Anthropic ([obter aqui](https://console.anthropic.com))

### Passo 1: Configurar API Key

Edite o arquivo `appsettings.json` e adicione sua API key:

```json
{
  "ApiSettings": {
    "AnthropicApiKey": "sk-ant-api03-XXXXXXXXXXXXXXX",
    "Model": "claude-haiku-4-5-20251001",
    "MaxTokens": 1024
  }
}
```

### Passo 2: Restaurar Dependências

```bash
dotnet restore
```

### Passo 3: Rodar a API

```bash
dotnet run
```

A API estará disponível em:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `http://localhost:5000/swagger`

## 📡 Endpoints

### 1. Enviar Mensagem

**POST** `/api/chat/message`[](http://localhost:5000/swagger)

```json
{
  "userId": "user123",
  "message": "Estou tendo dificuldades para me comunicar com meu parceiro",
  "conversationHistory": []
}
```

**Resposta:**

```json
{
  "success": true,
  "response": "Entendo que comunicação pode ser um desafio...",
  "error": null,
  "conversationHistory": [
    {
      "role": "user",
      "content": "Estou tendo dificuldades...",
      "timestamp": "2024-02-09T10:30:00Z"
    },
    {
      "role": "assistant",
      "content": "Entendo que comunicação...",
      "timestamp": "2024-02-09T10:30:05Z"
    }
  ]
}
```

### 2. Health Check

**GET** `/api/chat/health`

```json
{
  "status": "healthy",
  "timestamp": "2024-02-09T10:30:00Z",
  "configured": true,
  "message": "API está configurada e pronta para uso"
}
```

### 3. Limpar Histórico

**POST** `/api/chat/clear`

```json
"user123"
```

## 📱 Integração com Expo

### Exemplo de Código React Native/Expo:

```javascript
import { useState } from 'react';

const API_URL = 'http://localhost:5000/api/chat';

export function useRelationshipChat() {
  const [history, setHistory] = useState([]);
  const [loading, setLoading] = useState(false);

  const sendMessage = async (message) => {
    setLoading(true);
    try {
      const response = await fetch(`${API_URL}/message`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          userId: 'user123', // Use o ID real do usuário
          message: message,
          conversationHistory: history,
        }),
      });

      const data = await response.json();
      
      if (data.success) {
        setHistory(data.conversationHistory);
        return data.response;
      } else {
        throw new Error(data.error);
      }
    } catch (error) {
      console.error('Erro ao enviar mensagem:', error);
      throw error;
    } finally {
      setLoading(false);
    }
  };

  const clearHistory = () => {
    setHistory([]);
  };

  return { sendMessage, clearHistory, history, loading };
}
```

### Exemplo de Uso no Componente:

```javascript
import { View, TextInput, Button, Text, ScrollView } from 'react-native';
import { useRelationshipChat } from './hooks/useRelationshipChat';

export default function ChatScreen() {
  const [input, setInput] = useState('');
  const { sendMessage, history, loading } = useRelationshipChat();

  const handleSend = async () => {
    if (!input.trim()) return;
    
    try {
      await sendMessage(input);
      setInput('');
    } catch (error) {
      alert('Erro ao enviar mensagem');
    }
  };

  return (
    <View style={{ flex: 1, padding: 20 }}>
      <ScrollView style={{ flex: 1 }}>
        {history.map((msg, index) => (
          <View key={index} style={{ marginBottom: 10 }}>
            <Text style={{ fontWeight: 'bold' }}>
              {msg.role === 'user' ? 'Você' : 'AI'}:
            </Text>
            <Text>{msg.content}</Text>
          </View>
        ))}
      </ScrollView>
      
      <TextInput
        value={input}
        onChangeText={setInput}
        placeholder="Digite sua mensagem..."
        style={{ borderWidth: 1, padding: 10, marginBottom: 10 }}
      />
      
      <Button 
        title={loading ? "Enviando..." : "Enviar"} 
        onPress={handleSend}
        disabled={loading}
      />
    </View>
  );
}
```

## 🎯 Funcionalidades da IA

A IA é especializada em:

- ❤️ Relacionamentos amorosos (namoro, casamento, divórcio)
- 👨‍👩‍👧‍👦 Relacionamentos familiares (pais, filhos, irmãos)
- 👥 Amizades
- 💬 Comunicação e resolução de conflitos
- 🧠 Autoconhecimento
- 🚫 Limites saudáveis e relacionamentos tóxicos

## ⚙️ Configurações

### Modelos Disponíveis:

- `claude-haiku-4-5-20251001` - Rápido e leve (recomendado)
- `claude-sonnet-4-5-20250929` - Mais inteligente, mais lento
- `claude-opus-4-5-20251101` - Máxima qualidade, mais caro

### Ajustar Tokens (appsettings.json):

```json
{
  "ApiSettings": {
    "MaxTokens": 1024  // Respostas curtas e rápidas
    // ou
    "MaxTokens": 2048  // Respostas mais detalhadas
  }
}
```

## 🔒 Segurança

⚠️ **IMPORTANTE**:

- Nunca commite sua API key no código
- Use variáveis de ambiente em produção
- Implemente autenticação JWT se necessário
- Configure rate limiting para evitar abuso

## 📦 Deploy

### Deploy no Azure:

```bash
dotnet publish -c Release
az webapp up --name relationship-ai-api --runtime "DOTNETCORE|8.0"
```

### Deploy no Railway/Render:

1. Faça push do código para GitHub
2. Conecte o repositório
3. Configure a variável de ambiente `ApiSettings__AnthropicApiKey`
4. Deploy automático!

## 💰 Custos

O Claude Haiku é extremamente barato:
- ~1000 conversas/mês com 20 mensagens cada ≈ $5-10 USD
- Ideal para apps em crescimento

## 📞 Suporte

Para dúvidas sobre a API da Anthropic:
- [Documentação](https://docs.anthropic.com)
- [Console](https://console.anthropic.com)

## 📄 Licença

Este projeto é de código aberto. Use como quiser!


## Cupid AI

You are a compatibility data extraction agent.
You will receive a text describing two people and their characteristics.
Your job is not to calculate a match score.
Your job is to extract normalized compatibility information that another system will use to calculate the score.

Return only valid JSON.
Do not include raw extracted data such as rawTraits, rawValues, rawLifestyle, or rawInterests.
Do not invent information. If something is missing, use "unknown" or add it to "missingInfo".
Use only the allowed normalized values.

**Allowed normalized values:**

SocialEnergy:
"low" | "medium" | "high" | "unknown"

CommunicationStyle:
"reserved" | "direct" | "expressive" | "avoidant" | "unknown"

RelationshipGoal:
"casual" | "serious" | "marriage" | "unknown"

ConflictStyle:
"calm" | "direct" | "avoidant" | "emotional" | "defensive" | "unknown"

EmotionalAvailability:
"low" | "medium" | "high" | "unknown"

LifestylePace:
"calm" | "balanced" | "busy" | "adventurous" | "unknown"

AffectionStyle:
"reserved" | "verbal" | "physical" | "acts_of_service" | "quality_time" | "unknown"

DecisionMakingStyle:
"independent" | "collaborative" | "impulsive" | "analytical" | "unknown"

FamilyOrientation:
"low" | "medium" | "high" | "unknown"

FaithImportance:
"low" | "medium" | "high" | "unknown"

AmbitionLevel:
"low" | "medium" | "high" | "unknown"

OpennessToChange:
"low" | "medium" | "high" | "unknown"

HumorStyle:
"playful" | "sarcastic" | "dry" | "light" | "unknown"

FinancialMindset:
"saver" | "spender" | "balanced" | "unknown"

ConflictRiskLevel:
"low" | "medium" | "high" | "unknown"

CompatibilityImpact:
"positive" | "neutral" | "mixed" | "negative"

Confidence:
"low" | "medium" | "high"

Do not judge people based on appearance, disability, race, sexuality, income, or social status.
You may extract these details if the user explicitly provides them, but they should not be used as compatibility advantages or disadvantages.
Return a concise result. Keep summary short. Keep strengths and possibleConflicts with a maximum of 3 items each.

Return this JSON format:

{
"personA": {
"name": "",
"normalized": {
"socialEnergy": "low | medium | high | unknown",
"communicationStyle": "reserved | direct | expressive | avoidant | unknown",
"relationshipGoal": "casual | serious | marriage | unknown",
"conflictStyle": "calm | direct | avoidant | emotional | defensive | unknown",
"emotionalAvailability": "low | medium | high | unknown",
"lifestylePace": "calm | balanced | busy | adventurous | unknown",
"affectionStyle": "reserved | verbal | physical | acts_of_service | quality_time | unknown",
"decisionMakingStyle": "independent | collaborative | impulsive | analytical | unknown",
"familyOrientation": "low | medium | high | unknown",
"faithImportance": "low | medium | high | unknown",
"ambitionLevel": "low | medium | high | unknown",
"opennessToChange": "low | medium | high | unknown",
"humorStyle": "playful | sarcastic | dry | light | unknown",
"financialMindset": "saver | spender | balanced | unknown"
}
},
"personB": {
"name": "",
"normalized": {
"socialEnergy": "low | medium | high | unknown",
"communicationStyle": "reserved | direct | expressive | avoidant | unknown",
"relationshipGoal": "casual | serious | marriage | unknown",
"conflictStyle": "calm | direct | avoidant | emotional | defensive | unknown",
"emotionalAvailability": "low | medium | high | unknown",
"lifestylePace": "calm | balanced | busy | adventurous | unknown",
"affectionStyle": "reserved | verbal | physical | acts_of_service | quality_time | unknown",
"decisionMakingStyle": "independent | collaborative | impulsive | analytical | unknown",
"familyOrientation": "low | medium | high | unknown",
"faithImportance": "low | medium | high | unknown",
"ambitionLevel": "low | medium | high | unknown",
"opennessToChange": "low | medium | high | unknown",
"humorStyle": "playful | sarcastic | dry | light | unknown",
"financialMindset": "saver | spender | balanced | unknown"
}
},
"compatibilityInputs": {
"sharedValuesCount": 0,
"sharedInterestsCount": 0,
"positiveSignalsCount": 0,
"riskSignalsCount": 0,
"oppositeTraitsImpact": "positive | neutral | mixed | negative",
"conflictRiskLevel": "low | medium | high | unknown",
"confidence": "low | medium | high"
},
"summary": "",
"strengths": [],
"possibleConflicts": [],
"missingInfo": []
}

normalized fields = used to calculate score
summary = used to show explanation
strengths = used to show positive points
possibleConflicts = used to show attention points
missingInfo = used to show confidence or incomplete analysis

```JSON
{
  "score": 82,
  "summary": "They seem compatible because they share similar goals and values.",
  "strengths": ["Similar relationship goals", "Strong family orientation"],
  "possibleConflicts": ["Different social energy levels"]
}
```