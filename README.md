Folklore is a language that speaks tales.

It follows node-based paradigm, ensures graphics context and is easy to embed.

Each node consists of a set of inputs, outputs and execution logic.

A node supports CPU and GPU executin logic. Folklore compiles into host native code and GPU shaders.

## The goal

Folklore was designed to run PixiEditor Node Graphs outside PixiEditor. 
However, it's design is application-agnostic and the aim of the language is to be able to easily create GPU-accelerated graphics without the struggle.

## Folklore node concept

```
input num someNum;
output painter result;

num scale;

// on node execute
void execute()
{
    num x = 4;
    num y = 10;
    scale = x + y;
}

// result draw func
result_paint(gpu_ctx)
{
    gpu_ctx.drawCircle(0, 0, scale, scale);
}
```