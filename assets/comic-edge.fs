/*{
  "DESCRIPTION": "Posterised colour with inked outlines traced from the footage \u2014 a clean comic/cel-shaded look with no model required.",
  "CATEGORIES": ["Guillotine", "Stylize"],
  "INPUTS": [
  {
    "NAME": "inputImage",
    "TYPE": "image"
  },
  {
    "NAME": "levels",
    "TYPE": "float",
    "DEFAULT": 5.0,
    "MIN": 2.0,
    "MAX": 12.0
  },
  {
    "NAME": "ink",
    "TYPE": "float",
    "DEFAULT": 0.7,
    "MIN": 0.0,
    "MAX": 1.0
  }
]
}*/
void main() {
  vec2 uv = isf_FragNormCoord;
  vec2 t = 1.0 / RENDERSIZE;
  vec4 c = IMG_THIS_PIXEL(inputImage);

  // Quantise colour into flat bands — the cel-shading half.
  float n = max(floor(levels), 2.0);
  vec3 flat3 = floor(c.rgb * n + 0.5) / n;

  // Trace edges from the ORIGINAL (not the posterised) image so outlines follow real detail
  // rather than the banding boundaries the quantisation just introduced.
  vec3 l = IMG_NORM_PIXEL(inputImage, uv - vec2(t.x, 0.0)).rgb;
  vec3 r = IMG_NORM_PIXEL(inputImage, uv + vec2(t.x, 0.0)).rgb;
  vec3 u = IMG_NORM_PIXEL(inputImage, uv - vec2(0.0, t.y)).rgb;
  vec3 d = IMG_NORM_PIXEL(inputImage, uv + vec2(0.0, t.y)).rgb;
  float g = length(r - l) + length(d - u);
  float outline = 1.0 - smoothstep(0.12, 0.42, g) * ink;

  gl_FragColor = vec4(flat3 * outline, c.a);
}
